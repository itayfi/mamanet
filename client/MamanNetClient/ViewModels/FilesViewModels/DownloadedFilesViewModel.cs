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

namespace ViewModel.FilesViewModels
{
    public class DownloadedFilesViewModel:INotifyPropertyChanged
    {
        #region Public Fields

        public ObservableCollection<MamanNetFile> DownloadedFiles { get; set; }
        public RelayCommand<string> AddNewDownloadedFileCommand { get; set; }
        public RelayCommand<MamanNetFile> RemoveExistingDownloadedFileCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MamanNetFile DownloadedSelectedFile
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

        private readonly ObservableCollection<MamanNetFile> _allFiles;
        private MamanNetFile _selectedFile;
        private readonly DataStoreProvider _dataStoreProvider;

        #endregion

        #region Methods
        public DownloadedFilesViewModel(ObservableCollection<MamanNetFile> allFiles)
        {
            _allFiles = allFiles;
            _dataStoreProvider = new DataStoreProvider();
            DownloadedFiles = new ObservableCollection<MamanNetFile>();
            AddDownloadedFile(new MamanNetFile() { BytesDownloaded = 1, DownloadStatus = DownloadStatus.Failed, FileSizeInBytes = 1, FinishedPercentage = 99, Id = "aas", Leechers = 4, Seeders = 4, Name = "hello world", Type = FileType.Word });
            GetDownloadedFiles();
            AddNewDownloadedFileCommand = new RelayCommand<string>(AddPath);
            RemoveExistingDownloadedFileCommand = new RelayCommand<MamanNetFile>(_deleteDownloadedFile, _canDeleteDownloadFile);
        }
        public void SavedDownloadedFiles()
        {
            _dataStoreProvider.SaveData(DownloadedFiles.ToList());
        }

        public void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void AddDownloadedFile(MamanNetFile file)
        {
            DownloadedFiles.Add(file);
            _allFiles.Add(file);
        }

        private void AddPath(string file)
        {

        }

        private void _deleteDownloadedFile(MamanNetFile file)
        {
            DownloadedFiles.Remove(file);
            _allFiles.Remove(file);
        }

        private bool _canDeleteDownloadFile(MamanNetFile file)
        {
            if (file == null)
                return false;
            return true;
        }

        private void GetDownloadedFiles()
        {
            var dataStore = _dataStoreProvider.LoadData();
            foreach (var serializedFile in dataStore.SavedDataFiles)
            {
                var mamanNetFile = new MamanNetFile(serializedFile);
                AddDownloadedFile(mamanNetFile);
            }
        }

        #endregion
    }
}
