using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Models;

namespace ViewModel.FilesViewModels
{
    public class DownloadedFilesViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<MamanNetFile> DownloadedFiles { get; set; }
        private ObservableCollection<MamanNetFile> _allFiles;
        private Sqlite _dbHandler;

        private void AddDownloadedFile(MamanNetFile file)
        {
            DownloadedFiles.Add(file);
            _allFiles.Add(file);
        }

        private void DeleteDownloadedFile(MamanNetFile file)
        {
            DownloadedFiles.Remove(file);
            _allFiles.Remove(file);
        }

        public DownloadedFilesViewModel(ObservableCollection<MamanNetFile> allFiles)
        {
            _allFiles = allFiles;
            DownloadedFiles = new ObservableCollection<MamanNetFile>();
            AddDownloadedFile(new MamanNetFile() { BytesDownloaded = 1, DownloadStatus = DownloadStatus.Failed, FileSizeInBytes = 1, FinishedPercentage = 99, ID = "aas", Leechers = 4, Seeders = 4, Name = "hello world", Type = FileType.Word });
            _dbHandler = new Sqlite();
            GetDownloadedFiles();
        }

        private void GetDownloadedFiles()
        {
            _dbHandler.Connect();
            var files = _dbHandler.GetAllSavedFiles();
            foreach (var file in files)
            {
                AddDownloadedFile(file);
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
    }
}
