using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ViewModel.Files
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
        private ObservableCollection<MamanNetFile> _allFiles { get; set; }
        private int _downloadSpeed;

        #endregion

        #region Methods
        public DownloadingFilesViewModel(ObservableCollection<MamanNetFile> allFiles)
        {
            _allFiles = allFiles;
            DownloadingFiles = new ObservableCollection<MamanNetFile>();
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

        #endregion
    }
}
