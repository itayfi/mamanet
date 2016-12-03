using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Models;

namespace ViewModel.Files
{
    public class AllFilesViewModel:INotifyPropertyChanged
    {
        #region Public Fields

        public ObservableCollection<MamanNetFile> AllFilesCollection { get; set; }
        public DownloadingFilesViewModel DownloadingFilesViewModel { get; set; }
        public UploadingFilesViewModel UploadingFilesViewModel { get; set; }
        public DownloadedFilesViewModel DownloadedFilesViewModel { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Fields

        private readonly DataStoreProvider _dataStoreProvider;

        #endregion

        #region Methods

        public AllFilesViewModel()
        {
            AllFilesCollection = new ObservableCollection<MamanNetFile>();
            _dataStoreProvider = new DataStoreProvider();
            LoadSavedFiles();

            DownloadingFilesViewModel = new DownloadingFilesViewModel(AllFilesCollection);
            UploadingFilesViewModel = new UploadingFilesViewModel(AllFilesCollection);
            DownloadedFilesViewModel = new DownloadedFilesViewModel(AllFilesCollection);
        }

        public void SavedDownloadedFiles()
        {
            _dataStoreProvider.SaveData(AllFilesCollection.ToList());
        }
        private void LoadSavedFiles()
        {
            var dataStore = _dataStoreProvider.LoadData();
            foreach (var serializedFile in dataStore.SavedDataFiles)
            {
                var mamanNetFile = new MamanNetFile(serializedFile);
                AllFilesCollection.Add(mamanNetFile);
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

        #endregion
    }
}
