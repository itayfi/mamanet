using System;
using System.IO;
using Networking.Exceptions;

namespace Networking.Files
{
    [Serializable]
    public class FilePart :ICloneable
    {
        public FilePart(MamaNetFile mamaNetFile, int partNumber)
        {
            MamaNetFile = mamaNetFile;
            PartNumber = partNumber;
            IsPartAvailable = false;
        }

        #region Members
        public MamaNetFile MamaNetFile
        {
            get;
            private set;
        }

        public int PartNumber
        {
            get;
            private set;
        }

        public bool IsPartAvailable
        {
            get;
            internal set;
        }

        #endregion

        #region Methods

        private void GoToPart(FileStream stream)
        {
            stream.Seek(MamaNetFile.PartSize * PartNumber, SeekOrigin.Begin);
        }

        public byte[] GetData()
        {
            if (!IsPartAvailable)
            {
                throw new NotAvailableException(string.Format("The part {0} of MamaNetFile {0} was not downloaded yet", PartNumber, MamaNetFile));
            }

            var partSize = MamaNetFile.PartSize;
            byte[] buffer = new byte[partSize];

            using (FileStream stream = MamaNetFile.GetReadStream())
            {
                GoToPart(stream);

                if (PartNumber == MamaNetFile.NumberOfParts - 1)
                {
                    partSize = (int)(stream.Length - stream.Position);
                }
                
                stream.Read(buffer, 0, partSize);    
            }
            
            return buffer;
        }

        public void SetData(byte[] data)
        {
            if (data.Length != MamaNetFile.PartSize)
            {
                if (PartNumber != MamaNetFile.NumberOfParts - 1 || data.Length > MamaNetFile.PartSize)
                {
                    throw new MalformedDataException("Part data size should be same as MamaNetFile part size");
                }
            }

            //probably already downloaded the file from someone else
            if (IsPartAvailable) return;
            
            if (MamaNetFile._writeLock == null) {
                // When a MamanNetFile is created from serialization, it will not have a write lock :(
                MamaNetFile._writeLock = new object();
            }
            lock (MamaNetFile._writeLock)
            {
                using (var stream = MamaNetFile.GetWriteStream())
                {
                    GoToPart(stream);
                    stream.Write(data, 0, data.Length);
                    IsPartAvailable = true;
                    MamaNetFile.UpdateAvailability();    
                }
            }
        }

        public object Clone()
        {
            var other = new FilePart(MamaNetFile, PartNumber) { IsPartAvailable = IsPartAvailable };
            return other;
        }

        #endregion

    }
}