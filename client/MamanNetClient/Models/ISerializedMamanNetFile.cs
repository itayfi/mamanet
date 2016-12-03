using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Models
{
    public interface ISerializedMamanNetFile
    {
        //MD5 of the File
        string Id { get; set; }
        string Name { get; set; }
        FileType Type { get; set; }
        int FileSizeInBytes { get; set; }
        DownloadStatus DownloadStatus { get; set; }
    }
}
