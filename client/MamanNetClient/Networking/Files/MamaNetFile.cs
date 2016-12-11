using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Networking.Network;
using Networking.Utilities;

namespace Networking.Files
{
    public enum DownloadStatus
    {
        Downloading,
        DownladingError,
        Downloaded,
        DownloadFailed,
        Paused
    }

    public enum FileType
    {
        Image,
        Pdf,
        Word,
        Generic
    }

    [Serializable]
    public class MamaNetFile : MetadataFile
    {
        #region Private Members

        private readonly FilePart[] _parts;
        private string _localPath;
        private decimal _availability;
        private bool _isActive;
        [NonSerialized]
        internal FileStream _readStream;
        [NonSerialized]
        internal FileStream _writeStream;
        [NonSerialized]
        private IEnumerable<PeerDetails> _peers;
        [NonSerialized]
        internal object writeLock = new Object();

        #endregion

        #region Ctors

        public MamaNetFile(string fullName, byte[] expectedHash, string localPath, int totalSize, int partSize = 1024, string[] relatedHubs = null, bool isAvailable = false)
            : base(fullName, expectedHash, relatedHubs, totalSize, partSize)
        {
            this._localPath = localPath;
            this._parts = new FilePart[this.NumberOfParts];
            if (isAvailable)
            {
                for (int i = 0; i < NumberOfParts; i++)
                {
                    _parts[i] = new FilePart(this, i) { IsPartAvailable = true };
                }
                UpdateAvailability();
            }
        }

        public MamaNetFile(string fullName, string hash, string localPath, int totalSize, int partSize = 1024, bool isAvailable = false)
            : this(fullName, HexConverter.HexStringToByteArray(hash), localPath, totalSize, partSize, isAvailable: isAvailable)
        {
        }

        public MamaNetFile(MamaNetFile other)
            : base(other)
        {
            _localPath = other._localPath;
            _parts = other._parts;
        }

        //TODO: 2 ctors with same signiture! remove one of them
        public MamaNetFile(MetadataFile other) : base(other)
        {
            _parts = new FilePart[this.NumberOfParts];
        }

        #endregion

        #region Serialized Members

        public string LocalPath
        {
            get
            {
                return _localPath;
            }
            private set
            {
                _localPath = value;
                FireChangeEvent("LocalPath");
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                FireChangeEvent("IsActive");
                FireChangeEvent("DownloadStatus");
            }
        }
        #endregion

        #region Calculated Members

        public FilePart this[int number]
        {
            get
            {
                if (_parts[number] == null)
                {
                    _parts[number] = new FilePart(this, number);
                }
                return _parts[number];
            }
        }

        public decimal Availability
        {
            set
            {
                _availability = value;
                FireChangeEvent("Availability");
                FireChangeEvent("DownloadStatus");
            }
            get
            {
                return _availability;
            }
        }
        public DownloadStatus DownloadStatus
        {
            get
            {
                if (IsActive)
                {
                    //TODO: change this to find online RelatedHubs only!
                    return RelatedHubs.Any() ? DownloadStatus.Downloading : DownloadStatus.DownladingError;
                }
                else
                {
                    if (Availability == 1)
                    {
                        if (ExpectedHash == File.ReadAllBytes(LocalPath))
                        {
                            return DownloadStatus.Downloaded;
                        }
                        return DownloadStatus.DownloadFailed;
                    }
                    else
                    {
                        return DownloadStatus.Paused;
                    }
                }
            }
        }

        public FileType Type
        {
            get
            {
                var extention = FullName.Split('.').Last();
                switch (extention)
                {
                    case "jpeg":
                    case "jpg":
                        return FileType.Image;
                    case "pdf":
                        return FileType.Pdf;
                    case "doc":
                    case "docx":
                        return FileType.Word;
                    default:
                        //Todo: support generic file icon
                        return FileType.Generic;
                }
            }
        }

        public IEnumerable<PeerDetails> Peers
        {
            get
            {
                return _peers;
            }
            set
            {
                _peers = value;
                FireChangeEvent("Peers");
            }
        }
        #endregion

        #region Methods

        public void Close()
        {
            if (_readStream != null)
            {
                //Todo: use dispose instead of close?
                _readStream.Close();
                _readStream = null;
            }
            if (_writeStream != null)
            {
                _writeStream.Close();
                _writeStream = null;
            }
        }

        public int[] GetMissingParts()
        {
            List<int> missingParts = new List<int>();

            for (var i = 0; i < _parts.Length; i++)
            {
                if (_parts[i] == null || !_parts[i].IsPartAvailable)
                {
                    missingParts.Add(i);
                }
            }

            return missingParts.ToArray();
        }

        internal FileStream GetReadStream()
        {
            //TODO: use StreamReader instead of FileStream
            return _readStream ??
                   (_readStream = File.Open(_localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }

        internal FileStream GetWriteStream()
        {
            //TODO: use StreamWriter instead of FileStream
            return _writeStream ??
                   (_writeStream = File.Open(_localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read));
        }

        internal void UpdateAvailability()
        {
            Availability = Convert.ToDecimal(_parts.Count(part => part != null && part.IsPartAvailable)) /
                    Convert.ToDecimal(NumberOfParts);
        }

        #endregion
    }
}
