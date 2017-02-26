using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DAL;
using System;
using Networking.Files;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Networking.Network;

namespace ViewModels.Files
{
    public class AllFilesViewModel : BaseFilesViewModel, INotifyFileChange
    {
        #region Public Fields
       
        public DownloadingFilesViewModel DownloadingFilesViewModel { get; set; }
        public DownloadedFilesViewModel DownloadedFilesViewModel { get; set; }

        #endregion

        #region Private Fields

        private readonly DataStoreProvider _dataStoreProvider;
        private NetworkController _networkController;

        #endregion

        #region Methods

        public AllFilesViewModel()
        {
            var downloadDirectory = ConfigurationManager.AppSettings["DonwloadFolderPath"];
            if(!Directory.Exists(downloadDirectory))
                Directory.CreateDirectory(downloadDirectory);

            _dataStoreProvider = new DataStoreProvider();
            LoadSavedFiles();
            RelevatFilesCollection = AllFiles;
            DownloadingFilesViewModel = new DownloadingFilesViewModel(AllFiles);
            DownloadedFilesViewModel = new DownloadedFilesViewModel(AllFiles);

            _networkController = new NetworkController(AllFiles);
            Task.Run(() => _networkController.StartListen());
        }

        public override void AddFile(MamaNetFile file)
        {
            AllFiles.Add(file);
        }

        public override void AddFile(MetadataFile file)
        {
            AllFiles.Add(new MamaNetFile(file, ConfigurationManager.AppSettings["DonwloadFolderPath"]));
        }

        public override void _deleteFile()
        {
            AllFiles.Remove(SelectedFile);
        }

        public override bool _canDeleteFile()
        {
            return false;
        }

        public override bool _canAddFile(string filePath)
        {
            return false;
        }

        public void SavedDownloadedFiles()
        {
            _dataStoreProvider.SaveData(AllFiles.ToList());
        }

        private void LoadSavedFiles()
        {
            var dataStore = _dataStoreProvider.LoadData();
            foreach (var serializedFile in dataStore.SavedDataFiles)
            {
                AllFiles.Add(serializedFile);
            }
        }

        #endregion

        public override void FilterAllFilesToCollectionFiles()
        {
            //Intentionally does nothing
            return;
        }

        public void NotifyFileChange()
        {
            DownloadedFilesViewModel.FilterAllFilesToCollectionFiles();
            DownloadingFilesViewModel.FilterAllFilesToCollectionFiles();
        }
    }
}
