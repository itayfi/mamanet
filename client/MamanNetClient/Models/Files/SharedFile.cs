using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Networking;
using System.ComponentModel;

namespace Models.Files
{
    public enum DownloadStatus
    {
        Paused,
        Downloading,
        Downloaded,
        Uploading,
        Failed
    }

    [Serializable]
    public class SharedFile : MamanetFile, INotifyPropertyChanged
    {
        private FilePart[] parts;
        private string _localPath;
        private bool _isAvailable;
        private bool _isActive;
        [NonSerialized]
        internal FileStream inputStream;
        [NonSerialized]
        internal FileStream outputStream;

        #region Ctors
        public SharedFile(string name, byte[] hash, string localPath, int totalSize, int partSize = 1024, bool isAvailable = false, string[] hubs = null)
            : base(name, hash, hubs, totalSize, partSize)
        {
            this._localPath = localPath;
            this.parts = new FilePart[this.NumberOfParts];
            this.IsAvailable = isAvailable;
        }

        public SharedFile(string name, string hash, string localPath, int totalSize, int partSize = 1024, bool isAvailable = false)
            : this(name, Utils.HexStringToByteArray(hash), localPath, totalSize, partSize, isAvailable)
        {
        }

        public SharedFile(SharedFile other)
            : base(other)
        {
            _localPath = other._localPath;
            parts = other.parts.Select(p => (FilePart)p.Clone()).ToArray();
            IsAvailable = other.IsAvailable;
        }

        public SharedFile(MamanetFile other) : base(other)
        {
            parts = new FilePart[this.NumberOfParts];
        }
        #endregion

        public FilePart this[int number]
        {
            get
            {
                if (parts[number] == null)
                {
                    parts[number] = new FilePart(this, number);
                }
                return parts[number];
            }
        }

        #region Serialized Properties
        public string LocalPath
        {
            get { return _localPath; }
            private set
            {
                _localPath = value;
                FireChangeEvent("LocalPath");
            }
        }
        public bool IsAvailable
        {
            get { return _isAvailable; }
            private set
            {
                _isAvailable = value;
                FireChangeEvent("IsAvailable");
                FireChangeEvent("Availability");
                FireChangeEvent("DownloadStatus");
            }
        }
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                FireChangeEvent("IsActive");
                FireChangeEvent("DownloadStatus");
            }
        }
        #endregion
        #region Calculated Properties
        public decimal Availability
        {
            get
            {
                return Convert.ToDecimal(parts.Count(part => part != null ? part.IsAvailable : false)) / 
                    Convert.ToDecimal(NumberOfParts);
            }
        }
        public DownloadStatus DownloadStatus
        {
            get
            {
                if (IsActive)
                {
                    return IsAvailable ? DownloadStatus.Uploading : DownloadStatus.Downloading;
                }
                if (Availability == 0)
                {
                    return DownloadStatus.Paused;
                }
                if (IsAvailable)
                {
                    return DownloadStatus.Downloaded;
                }
                return DownloadStatus.Failed;
            }
        }
        public string Type
        {
            get
            {
                return Name.Split('.').Last();
            }
        }
        #endregion
        #region Methods
        internal FileStream GetInputStream()
        {
            if (this.inputStream == null)
            {
                this.inputStream = File.Open(_localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            return this.inputStream;
        }
        internal FileStream GetOutputStream()
        {
            if (this.outputStream == null)
            {
                this.outputStream = File.Open(_localPath, FileMode.OpenOrCreate, FileAccess.Write, 
                    FileShare.Read);
            }
            return this.outputStream;
        }
        public void Close()
        {
            if (inputStream != null)
            {
                inputStream.Close();
                inputStream = null;
            }
            if (outputStream != null)
            {
                outputStream.Close();
                outputStream = null;
            }
        }
        internal void UpdateAvailability()
        {
            IsAvailable = IsAvailable || parts.All(part => part != null ? part.IsAvailable : false);
        }
        #endregion

        #region INotifyPropertyChanged
        private void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
