using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum FileType
    {
        Image,
        Pdf,
        Word
    }

    public enum DownloadStatus
    {
        NotStarted,
        Downloading,
        Downloaded,
        Failed
    }

    [Serializable]
    public class MamanNetFile : ISerializedMamanNetFile
    {
        //Serialized Fields
        public string ID { get; set; }
        public string Name { get; set; }
        public FileType Type { get; set; }
        public int FileSizeInBytes { get; set; }
        public DownloadStatus DownloadStatus { get; set; }

        public MamanNetFile(ISerializedMamanNetFile serializedFile)
        {
            ID = serializedFile.ID;
            Name = serializedFile.Name;
            Type = serializedFile.Type;
            FileSizeInBytes = serializedFile.FileSizeInBytes;
            DownloadStatus = serializedFile.DownloadStatus;
        }

        public MamanNetFile()
        {
            
        }

        //Calculated Fields
        public int BytesDownloaded { get; set; }
        public int FinishedPercentage { get; set; }
        public int Seeders { get; set; }
        public int Leechers { get; set; }
    }
}
