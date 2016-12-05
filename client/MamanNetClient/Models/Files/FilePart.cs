using Models.Exceptions;
using System;
using System.IO;

namespace Models.Files
{
    [Serializable]
    public class FilePart :ICloneable
    {
        public FilePart(MamaNetFile file, int number)
        {
            this.File = file;
            this.Number = number;
            this.IsAvailable = file.IsAvailable;
        }

        public MamaNetFile File
        {
            get; private set;
        }

        public int Number
        {
            get; private set;
        }

        public bool IsAvailable
        {
            get; private set;
        }

        private FileStream GoToPart(FileStream stream)
        {
            stream.Seek(File.PartSize * Number, SeekOrigin.Begin);
            return stream;
        }

        public byte[] GetData()
        {
            if (!IsAvailable)
            {
                throw new NotAvailableException(string.Format("The part {0} of file {0} was not downloaded yet", Number, File));
            }
            FileStream stream = File.GetInputStream();
            GoToPart(stream);
            byte[] buffer = new byte[File.PartSize];
            stream.Read(buffer, 0, File.PartSize);
            return buffer;
        }

        public void SetData(byte[] data)
        {
            if (data.Length != File.PartSize)
            {
                throw new MalformedDataException("Part data size should be same as file part size");
            }
            FileStream stream = File.GetOutputStream();
            GoToPart(stream);
            stream.Write(data, 0, File.PartSize);
            IsAvailable = true;
            File.UpdateAvailability();
        }

        public object Clone()
        {
            FilePart other = new FilePart(File, Number);
            other.IsAvailable = IsAvailable;
            return other;
        }
    }
}