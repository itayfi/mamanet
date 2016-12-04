using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Files;

namespace ViewModel.Files
{
    public class DownloadingFilesViewModel:INotifyPropertyChanged
    {
        #region Public Fields
        public ObservableCollection<SharedFile> DownloadingFiles { get; set; }

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
        private ObservableCollection<SharedFile> _allFiles { get; set; }
        private int _downloadSpeed;

        #endregion

        #region Methods
        public DownloadingFilesViewModel(ObservableCollection<SharedFile> allFiles)
        {
            _allFiles = allFiles;
            DownloadingFiles = new ObservableCollection<SharedFile>();
            FilterDownloadedingFiles();
            DownloadSpeed = 500;
        }

        private void FilterDownloadedingFiles()
        {
            var downloadedingFiles = _allFiles.Where(file => file.DownloadStatus == DownloadStatus.Downloading || file.DownloadStatus == DownloadStatus.Failed);
            foreach (var file in downloadedingFiles)
            {
                DownloadingFiles.Add(file);
            }
        }

        private void AddDownloadedFile(MamanetFile serializedMamanNetFile)
        {
            SharedFile sharedFile = serializedMamanNetFile as SharedFile;
            if (sharedFile == null)
            {
                sharedFile = new SharedFile(serializedMamanNetFile);
            }
            DownloadingFiles.Add(sharedFile);
            _allFiles.Add(sharedFile);
        }

        private void DeleteDownloadedFile(SharedFile serializedMamanNetFile)
        {
            DownloadingFiles.Remove(serializedMamanNetFile);
            _allFiles.Remove(serializedMamanNetFile);
        }

        #endregion
    }
}
