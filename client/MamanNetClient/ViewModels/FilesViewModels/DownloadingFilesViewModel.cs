using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ViewModel.FilesViewModels
{
    public class DownloadingFilesViewModel:INotifyPropertyChanged
    {
        #region Public Fields
        public ObservableCollection<MamanNetFile> DownloadingFiles { get; set; }

        public int DownloadSpeed
        {
            get
            {
                return _downloadSpeed;
            }
            set
            {
                _downloadSpeed = value;
                FireChangeEvent("DownloadSpeed");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Private Fields
        private ObservableCollection<MamanNetFile> AllFiles { get; set; }
        private int _downloadSpeed;

        #endregion

        #region Methods
        public DownloadingFilesViewModel(ObservableCollection<MamanNetFile> allFiles)
        {
            AllFiles = allFiles;
            DownloadingFiles = new ObservableCollection<MamanNetFile>();
            AddDownloadedFile(
                new MamanNetFile() { BytesDownloaded = 500, DownloadStatus = DownloadStatus.Downloading, FinishedPercentage = 33, Id = "sad-asd-22", FileSizeInBytes = 4202, Leechers = 4, Name = "MyFile", Seeders = 23, Type = FileType.Pdf });
            DownloadSpeed = 500;
        }
        private void AddDownloadedFile(MamanNetFile serializedMamanNetFile)
        {
            DownloadingFiles.Add(serializedMamanNetFile);
            AllFiles.Add(serializedMamanNetFile);
        }

        private void DeleteDownloadedFile(MamanNetFile serializedMamanNetFile)
        {
            DownloadingFiles.Remove(serializedMamanNetFile);
            AllFiles.Remove(serializedMamanNetFile);
        }

        #endregion
    }
}
