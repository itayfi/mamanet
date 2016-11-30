using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibMamanet.Files
{
    public class MamanetFile
    {
        private byte[] hash;
        private FilePart[] parts;
        internal FileInfo localFile;
        internal FileStream fileStream;

        public MamanetFile(string name, byte[] hash, FileInfo localFile, int totalSize, int partSize = 1024, bool isAvailable = false)
        {
            this.Name = name;
            this.hash = (byte[])hash.Clone();
            this.localFile = localFile;
            this.TotalSize = totalSize;
            this.PartSize = partSize;
            this.parts = new FilePart[this.NumberOfParts];
        }

        public MamanetFile(string name, string hash, string localPath, int totalSize, int partSize = 1024, bool isAvailable = false)
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
                return Convert.ToDecimal(parts.Count(part => part.IsAvailable)) / Convert.ToDecimal(NumberOfParts);
            }
        }
        internal FileStream GetStream()
        {
            if (this.fileStream == null)
            {
                this.fileStream = this.localFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            return this.fileStream;
        }
        internal void Close()
        {
            this.fileStream.Close();
        }
        internal void UpdateAvailability()
        {
            IsAvailable = parts.All(part => part.IsAvailable);
        }
    }
}
