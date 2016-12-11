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

            FileStream stream = MamaNetFile.GetReadStream();
            GoToPart(stream);
            byte[] buffer = new byte[MamaNetFile.PartSize];
            stream.Read(buffer, 0, MamaNetFile.PartSize);
            return buffer;
        }

        public void SetData(byte[] data)
        {
            if (data.Length != MamaNetFile.PartSize)
            {
                throw new MalformedDataException("Part data size should be same as MamaNetFile part size");
            }

            //probably already downloaded the file from someone else
            if (IsPartAvailable) return;

            var stream = MamaNetFile.GetWriteStream();
            lock (MamaNetFile.writeLock)
            {
                GoToPart(stream);
                stream.Write(data, 0, MamaNetFile.PartSize);
                IsPartAvailable = true;
                MamaNetFile.UpdateAvailability();
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