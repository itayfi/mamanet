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

    public class MamanNetFile:IFileRow
    {
        //MD5 of the File
        public string ID { get; set; }
        public string Name { get; set; }
        public FileType Type { get; set; }
        public int FileSizeInBytes { get; set; }
        public DownloadStatus DownloadStatus { get; set; }
        public int Leechers { get; set; }
        public int Seeders { get; set; }

        //Calculated Field
        public int BytesDownloaded { get; set; }
        //Calculated Field
        public int FinishedPercentage { get; set; }
    
    }
}
