using System;
using System.ComponentModel;

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
        Paused,
        Downloading,
        Downloaded,
        Failed
    }

    [Serializable]
    public class MamanNetFile : ISerializedMamanNetFile, INotifyPropertyChanged
    {
        #region Private Fields
        private string _id { get; set; }
        private string _name { get; set; }
        private FileType _fileType { get; set; }
        private int _fileSizeInBytes { get; set; }
        private int _bytesDownloaded { get; set; }
        private int _finishedPercentage { get; set; }
        private int _seeders { get; set; }
        private int _leechers { get; set; }
        private DownloadStatus _downloadStatus;

        #endregion

        #region Serialized Fields

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                FireChangeEvent("Id");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                FireChangeEvent("Name");
            }
        }

        public FileType Type
        {
            get { return _fileType; }
            set
            {
                _fileType = value;
                FireChangeEvent("Type");
            }
        }

        public int FileSizeInBytes
        {
            get { return _fileSizeInBytes; }
            set
            {
                _fileSizeInBytes = value;
                FireChangeEvent("FileSizeInBytes");
            }
        }

        public DownloadStatus DownloadStatus
        {
            get { return _downloadStatus; }
            set
            {
                _downloadStatus = value;
                FireChangeEvent("DownloadStatus");
            }
        }

        #endregion

        #region Calculated Fields

        public int BytesDownloaded
        {
            get { return _bytesDownloaded; }
            set
            {
                _bytesDownloaded = value;
                FireChangeEvent("BytesDownloaded");
            }
        }

        public int FinishedPercentage
        {
            get { return _finishedPercentage; }
            set
            {
                _finishedPercentage = value;
                FireChangeEvent("FinishedPercentage");
            }
        }

        public int Seeders
        {
            get { return _seeders; }
            set
            {
                _seeders = value;
                FireChangeEvent("Seeders");
            }
        }

        public int Leechers
        {
            get { return _leechers; }
            set
            {
                _leechers = value;
                FireChangeEvent("Leechers");
            }
        }

        #endregion

        #region Methods

        public MamanNetFile(ISerializedMamanNetFile serializedFile)
        {
            Id = serializedFile.Id;
            Name = serializedFile.Name;
            Type = serializedFile.Type;
            FileSizeInBytes = serializedFile.FileSizeInBytes;
            DownloadStatus = serializedFile.DownloadStatus;
        }

        public MamanNetFile()
        {

        }

        private void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
       [field: NonSerializedAttribute()] 
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
