using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using Networking.Network;
using Networking.Utilities;

namespace Networking.Files
{
    public enum FileStatus
    {
        Paused,
        DownloadFailure,
        Downloaded,
        Downloading,
        Uploading
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
        private ObservableCollection<PeerDetails> _peers;
        [NonSerialized] 
        internal object _writeLock;
        [NonSerialized]
        private byte[] _currentFileHash;
        [NonSerialized]
        private int _seeders;
        [NonSerialized]
        private int _leechers;
        private int _availableFileParts;
        private ObservableCollection<HubDetails> _relatedHubsDetails;

        #endregion

        #region Ctors

        public MamaNetFile(string fullName, byte[] expectedHash, string localPath, int totalSize, string[] relatedHubs, bool isFullAvailable = false, string description = "", int partSize = 1024)
            : base(fullName, expectedHash, relatedHubs, totalSize, partSize,description)
        {
            _writeLock = new object();
            _localPath = localPath;
            _parts = new FilePart[NumberOfParts];
            for (var i = 0; i < NumberOfParts; i++)
            {
                _parts[i] = new FilePart(this, i) { IsPartAvailable = isFullAvailable };
            }
            _currentFileHash = new byte[0];
            UpdateAvailability();
            DateAdded = DateTime.Now;
            Peers = new ObservableCollection<PeerDetails>();
            RelatedHubs = new ObservableCollection<HubDetails>();
            foreach (var hub in relatedHubs)
            {
                RelatedHubs.Add(new HubDetails(hub));
            }
        }

        public MamaNetFile(MetadataFile other, string folderPath)
            : this(other.FullName, other.ExpectedHash, Path.Combine(folderPath, other.FullName), totalSize: other.Size, relatedHubs: other.RelatedHubs, partSize: other.PartSize)
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

        public ObservableCollection<HubDetails> RelatedHubs
        {
            get { return _relatedHubsDetails; }
            set
            {
                _relatedHubsDetails = value;
                FireChangeEvent("RelatedHubs");
            }
        }


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

        public int AvailableFileParts
        {
            get { return _availableFileParts; }
            set
            {
                _availableFileParts = value;
                FireChangeEvent("AvailableFileParts");
            }
        }

        public FileStatus FileStatus
        {
            get
            {
                if (IsActive)
                {
                    if (Availability == 1)
                    {
                        if (ExpectedHash.SequenceEqual(_currentFileHash))
                        {
                            return FileStatus.Uploading;
                        }
                        else
                        {
                            return FileStatus.DownloadFailure;
                        }
                    }
                    else
                    {
                        return FileStatus.Downloading;
                    }
                }
                {
                    if (Availability == 1)
                    {
                        if (ExpectedHash.SequenceEqual(_currentFileHash))
                        {
                            return FileStatus.Downloaded;
                        }
                        else
                        {
                            return FileStatus.DownloadFailure;
                        }
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
                switch (extention.ToLower())
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
                        return FileType.Generic;
                }
            }
        }

        public ObservableCollection<PeerDetails> Peers
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

        public int Seeders
        {
            get
            {
                return _seeders;
            }
            set
            {
                _seeders = value;
                FireChangeEvent("Seeders");
            }
        }

        public int Leechers
        {
            get
            {
                return _leechers;
            }
            set
            {
                _leechers = value;
                FireChangeEvent("Leechers");
            }
        }

        public string Indexer { get; set; }

        #endregion

        #region Public Members

        public DateTime DateAdded { get; set; }

        public DateTime DateDownloaded { get; set; }

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
            if (_readStream == null || !_readStream.CanRead)
            {
                _readStream = File.Open(_localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

            return _readStream;
        }

        internal FileStream GetWriteStream()
        {
            if (_writeStream == null || !_writeStream.CanWrite)
            {
                _writeStream = File.Open(_localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            }

            return _writeStream;
        }

        internal void UpdateAvailability()
        {
            var availableFileParts = _parts.Count(part => part.IsPartAvailable);
            Availability = Convert.ToDecimal(availableFileParts) / Convert.ToDecimal(NumberOfParts);
            UpdateFileHash();

            if (Availability == 1)
            {
                FireChangeEvent("FileStatus");
            }

            AvailableFileParts = availableFileParts;
        }

        private void UpdateFileHash()
        {
            if(File.Exists(LocalPath))
            {
                using (var fileStream = File.Open(LocalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    _currentFileHash = HashUtils.CalculateHash(fileStream);
                }
            }
        }

        public void SyncPeersInformation(List<PeerDetails> peers, TaskScheduler syncContextScheduler)
        {
            Task.Factory.StartNew(() =>
            {
                Leechers = peers.Count;
                int seeders = 0;

                if(Peers == null)
                {
                    Peers = new ObservableCollection<PeerDetails>();
                }
                Peers.Clear();

                foreach (var peer in peers)
                {
                    if (peer.AvailableFileParts.Length == NumberOfParts)
                    {
                        seeders++;
                    }
                    Peers.Add(peer);
                }
                Seeders = seeders;
            }, CancellationToken.None, TaskCreationOptions.None, syncContextScheduler);
        }

        public void SyncHubInformation(HubDetails hubDetails, TaskScheduler syncContextScheduler)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var hub in RelatedHubs)
                {
                    if (hub.Url == hubDetails.Url)
                    {
                        hub.LastCommunicationTime = DateTime.Now;
                        hub.ConnectedUsers = hubDetails.ConnectedUsers;
                    }
                }

            }, CancellationToken.None, TaskCreationOptions.None, syncContextScheduler);
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
