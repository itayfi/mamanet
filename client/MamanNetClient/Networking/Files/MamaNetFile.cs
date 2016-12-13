﻿using System;
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
    public enum FileStatus
    {
        Uploading,
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
    public class MamaNetFile : MetadataFile, IDisposable
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
        [NonSerialized]
        private byte[] _currentFileHash;

        #endregion

        #region Ctors

        public MamaNetFile(string fullName, byte[] expectedHash, string localPath, int totalSize, int partSize = 1024, string[] relatedHubs = null, bool isFullAvailable = false)
            : base(fullName, expectedHash, relatedHubs, totalSize, partSize)
        {
            _localPath = localPath;
            _parts = new FilePart[NumberOfParts];
            for (var i = 0; i < NumberOfParts; i++)
            {
                _parts[i] = new FilePart(this, i) { IsPartAvailable = isFullAvailable };
            }
            _currentFileHash = new byte[0];
            UpdateAvailability();
        }

        public MamaNetFile(MetadataFile other,string folderPath) : this(other.FullName, other.ExpectedHash, Path.Combine(folderPath, other.FullName), other.Size, other.PartSize, other.RelatedHubs)
        {
            
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
                if (!_isActive)
                {
                    if (_writeStream != null) _writeStream.Dispose();
                    if (_readStream != null) _readStream.Dispose();
                }

                FireChangeEvent("IsActive");
                FireChangeEvent("FileStatus");
            }
        }
        #endregion

        #region Calculated Members

        public FilePart this[int number]
        {
            get
            {
                return _parts[number];
            }
        }

        public decimal Availability
        {
            set
            {
                _availability = value;
                FireChangeEvent("Availability");
                FireChangeEvent("FileStatus");
            }
            get
            {
                return _availability;
            }
        }
        public FileStatus FileStatus
        {
            get
            {
                if (IsActive)
                {
                    //TODO: change this to find online RelatedHubs only!
                    if (Availability == 1)
                    {
                        return FileStatus.Uploading;
                    }
                    else
                    {
                        return RelatedHubs.Any() ? FileStatus.Downloading : FileStatus.DownladingError;    
                    }
                    
                }
                else
                {
                    if (Availability == 1)
                    {
                        if (ExpectedHash.SequenceEqual(_currentFileHash))
                        {
                            return FileStatus.Downloaded;
                        }
                        return FileStatus.DownloadFailed;
                    }
                    else
                    {
                        return FileStatus.Paused;
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
                    case "png":
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

        public int[] GetMissingParts()
        {
            List<int> missingParts = new List<int>();

            for (var i = 0; i < _parts.Length; i++)
            {
                if (!_parts[i].IsPartAvailable)
                {
                    missingParts.Add(i);
                }
            }

            return missingParts.ToArray();
        }

        public int[] GetAvailableParts()
        {
            List<int> availableParts = new List<int>();

            for (var i = 0; i < _parts.Length; i++)
            {
                if (_parts[i].IsPartAvailable)
                {
                    availableParts.Add(i);
                }
            }

            return availableParts.ToArray();
        }

        internal FileStream GetReadStream()
        {
            return _readStream ??
                   (_readStream = File.Open(_localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }

        internal FileStream GetWriteStream()
        {
            return _writeStream ??
                   (_writeStream = File.Open(_localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read));
        }

        internal void UpdateAvailability()
        {
            Availability = Convert.ToDecimal(_parts.Count(part => part.IsPartAvailable)) /
                    Convert.ToDecimal(NumberOfParts);

            if (Availability == 1)
            {
                UpdateFileHash();
                if (_writeStream != null) _writeStream.Dispose();
            }
        }

        private void UpdateFileHash()
        {
            using (var fileStream = File.Open(LocalPath,FileMode.Open,FileAccess.Read, FileShare.ReadWrite))
            {
                _currentFileHash = HashUtils.CalculateHash(fileStream);
            }
        }

        #endregion

        public void Dispose()
        {
            if (_readStream != null)
            {
                _readStream.Close();
                _readStream = null;
            }
            if (_writeStream != null)
            {
                _writeStream.Close();
                _writeStream = null;
            }
        }
    }
}
