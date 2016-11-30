using LibMamanet.Exceptions;
using System.IO;

namespace LibMamanet.Files
{
    public class FilePart
    {
        public FilePart(MamanetFile file, int number)
        {
            this.File = file;
            this.Number = number;
            this.IsAvailable = file.IsAvailable;
        }

        public MamanetFile File
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

        private FileStream GoToPart()
        {
            FileStream stream = File.GetStream();
            stream.Seek(File.PartSize * Number, SeekOrigin.Begin);
            return stream;
        }

        public byte[] GetData()
        {
            if (!IsAvailable)
            {
                throw new NotAvailableException(string.Format("The part {0} of file {0} was not downloaded yet", Number, File));
            }
            FileStream stream = GoToPart();
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
            FileStream stream = GoToPart();
            stream.Write(data, 0, File.PartSize);
            IsAvailable = true;
            File.UpdateAvailability();
        }
    }
}