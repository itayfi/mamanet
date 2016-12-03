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
        //Public Fields
        public ObservableCollection<MamanNetFile> DownloadingFiles { get; set; }
        private ObservableCollection<MamanNetFile> _allFiles { get; set; }

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

        //Private Fields
        private int _downloadSpeed;



        public DownloadingFilesViewModel(ObservableCollection<MamanNetFile> allFiles)
        {
            _allFiles = allFiles;
            DownloadingFiles = new ObservableCollection<MamanNetFile>();
            AddDownloadedFile(
                new MamanNetFile() { BytesDownloaded = 500, DownloadStatus = DownloadStatus.Downloading, FinishedPercentage = 33, ID = "sad-asd-22", FileSizeInBytes = 4202, Leechers = 4, Name = "MyFile", Seeders = 23, Type = FileType.Pdf });
            DownloadSpeed = 500;
            
        }

        private void AddDownloadedFile(MamanNetFile serializedMamanNetFile)
        {
            DownloadingFiles.Add(serializedMamanNetFile);
            _allFiles.Add(serializedMamanNetFile);
        }

        private void DeleteDownloadedFile(MamanNetFile serializedMamanNetFile)
        {
            DownloadingFiles.Remove(serializedMamanNetFile);
            _allFiles.Remove(serializedMamanNetFile);
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
    }
}
