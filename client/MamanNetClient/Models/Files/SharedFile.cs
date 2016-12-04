using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Models.Files
{
    public enum DownloadStatus
    {
        NotStarted,
        Downloading,
        Downloaded,
        Uploading,
        Failed
    }

    [Serializable]
    public class SharedFile : MamanetFile
    {
        private FilePart[] parts;
        private string localPath;
        [NonSerialized]
        internal FileStream inputStream;
        [NonSerialized]
        internal FileStream outputStream;

        public SharedFile(string name, byte[] hash, string localPath, int totalSize, int partSize = 1024, bool isAvailable = false, string[] hubs = null)
            : base(name, hash, hubs, totalSize, partSize)
        {
            this.localPath = localPath;
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
            localPath = other.localPath;
            parts = other.parts.Select(p => (FilePart)p.Clone()).ToArray();
            IsAvailable = other.IsAvailable;
        }

        public SharedFile(MamanetFile other) : base(other)
        {
            parts = new FilePart[this.NumberOfParts];
        }

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

        public string LocalPath
        {
            get; private set;
        }
        public bool IsAvailable
        {
            get; private set;
        }
        public decimal Availability
        {
            get
            {
                return Convert.ToDecimal(parts.Count(part => part != null ? part.IsAvailable : false)) / 
                    Convert.ToDecimal(NumberOfParts);
            }
        }
        public bool IsActive
        {
            get; set;
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
                    return DownloadStatus.NotStarted;
                }
                if (IsAvailable)
                {
                    return DownloadStatus.Downloaded;
                }
                return DownloadStatus.Failed;
            }
        }
        internal FileStream GetInputStream()
        {
            if (this.inputStream == null)
            {
                this.inputStream = File.Open(localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            return this.inputStream;
        }
        internal FileStream GetOutputStream()
        {
            if (this.outputStream == null)
            {
                this.outputStream = File.Open(localPath, FileMode.OpenOrCreate, FileAccess.Write, 
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
    }
}
