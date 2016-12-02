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

        public int UploadSpeed
        {
            get
            {
                return _uploadSpeed;
            }
            set
            {
                _uploadSpeed = value;
                FireChangeEvent("UploadSpeed");
            }
        }

        //Private Fields
        private int _downloadSpeed;
        private int _uploadSpeed;
        

        public DownloadingFilesViewModel(ObservableCollection<MamanNetFile> allFiles)
        {
            _allFiles = allFiles;
            DownloadingFiles = new ObservableCollection<MamanNetFile>();
            AddDownloadedFile(
                new MamanNetFile(){BytesDownloaded = 500, DownloadStatus = DownloadStatus.Downloading,FinishedPercentage = 33,ID = "sad-asd-22",FileSizeInBytes = 4202,Leechers = 4,Name = "MyFile",Seeders = 23,Type = FileType.Pdf});
            DownloadSpeed = 500;
            UploadSpeed = 24;
        }

        private void AddDownloadedFile(MamanNetFile file)
        {
            DownloadingFiles.Add(file);
            _allFiles.Add(file);
        }

        private void DeleteDownloadedFile(MamanNetFile file)
        {
            DownloadingFiles.Remove(file);
            _allFiles.Remove(file);
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
