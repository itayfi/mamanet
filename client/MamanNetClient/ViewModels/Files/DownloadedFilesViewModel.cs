using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using GalaSoft.MvvmLight.Command;
using Models;
using Models.Files;

namespace ViewModel.Files
{
    public class DownloadedFilesViewModel:INotifyPropertyChanged
    {
        #region Public Fields

        public ObservableCollection<SharedFile> DownloadedFiles { get; set; }
        public RelayCommand<string> AddNewDownloadedFileCommand { get; set; }
        public RelayCommand<SharedFile> RemoveExistingDownloadedFileCommand { get; set; }
        public event EventHandler<string> ShowPopup;
        public event PropertyChangedEventHandler PropertyChanged;

        public SharedFile DownloadedSelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                FireChangeEvent("SelectedFile");
            }
        }

        #endregion

        #region Private Fields

        private readonly ObservableCollection<SharedFile> _allFiles;
        private SharedFile _selectedFile;

        #endregion

        #region Methods
        public DownloadedFilesViewModel(ObservableCollection<SharedFile> allFiles)
        {
            _allFiles = allFiles;
            DownloadedFiles = new ObservableCollection<SharedFile>();
            FilterDownloadedFiles();
            AddNewDownloadedFileCommand = new RelayCommand<string>(AddPath);
            RemoveExistingDownloadedFileCommand = new RelayCommand<SharedFile>(_deleteDownloadedFile, _canDeleteDownloadFile);
        }

        private void FilterDownloadedFiles()
        {
            var downloadedFiles = _allFiles.Where(file => file.DownloadStatus == DownloadStatus.Downloaded);
            foreach (var file in downloadedFiles)
            {
                DownloadedFiles.Add(file);
            }
        }

        public void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void AddDownloadedFile(SharedFile file)
        {
            DownloadedFiles.Add(file);
            _allFiles.Add(file);
        }

        private void AddPath(string filePath)
        {
            if (ShowPopup != null) ShowPopup(this, "קובץ התווסף בהצלחה!");
        }

        private void _deleteDownloadedFile(SharedFile file)
        {
            DownloadedFiles.Remove(file);
            _allFiles.Remove(file);
        }

        private bool _canDeleteDownloadFile(SharedFile file)
        {
            if (file == null)
                return false;
            return true;
        }

        #endregion
    }
}
