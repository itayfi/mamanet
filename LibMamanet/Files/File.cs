using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibMamanet.Files
{
    public class SharedFile
    {
        private byte[] hash;
        private FilePart[] parts;
        internal FileInfo localFile;
        internal FileStream inputStream;
        internal FileStream outputStream;

        public SharedFile(string name, byte[] hash, FileInfo localFile, int totalSize, int partSize = 1024, bool isAvailable = false)
        {
            this.Name = name;
            this.hash = (byte[])hash.Clone();
            this.localFile = localFile;
            this.TotalSize = totalSize;
            this.PartSize = partSize;
            this.parts = new FilePart[this.NumberOfParts];
            this.IsAvailable = isAvailable;
        }

        public SharedFile(string name, string hash, string localPath, int totalSize, int partSize = 1024, bool isAvailable = false)
            : this(name, Utils.HexStringToByteArray(hash), new FileInfo(localPath), totalSize, partSize, isAvailable)
        {
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

        public string Name
        {
            get;
            set;
        }
        public byte[] Hash
        {
            get
            {
                return (byte[])hash.Clone();
            }
            set
            {
                this.hash = (byte[])value.Clone();
            }
        }
        public int TotalSize
        {
            get;
            private set;
        }
        public int PartSize
        {
            get;
            private set;
        }
        public MamanetFile MamanetFile
        {
            get; set;
        }
        public int NumberOfParts
        {
            get
            {
                return Convert.ToInt32(Math.Ceiling(Convert.ToDouble(TotalSize) / Convert.ToDouble(PartSize)));
            }
        }
        public string LocalPath
        {
            get
            {
                return localFile.FullName;
            }
        }
        public bool IsAvailable
        {
            get; private set;
        }
        public decimal Availability
        {
            get
            {
                return Convert.ToDecimal(parts.Count(part => part != null ? part.IsAvailable : false)) / Convert.ToDecimal(NumberOfParts);
            }
        }
        internal FileStream GetInputStream()
        {
            if (this.inputStream == null)
            {
                this.inputStream = this.localFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            return this.inputStream;
        }
        internal FileStream GetOutputStream()
        {
            if (this.outputStream == null)
            {
                this.outputStream = this.localFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
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
            IsAvailable = parts.All(part => part != null ? part.IsAvailable : false);
        }
    }
}
