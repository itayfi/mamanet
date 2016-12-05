﻿using System;
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
    public class AllFilesViewModel:BaseFilesViewModel
    {
        #region Public Fields
       
        public DownloadingFilesViewModel DownloadingFilesViewModel { get; set; }
        public UploadingFilesViewModel UploadingFilesViewModel { get; set; }
        public DownloadedFilesViewModel DownloadedFilesViewModel { get; set; }

        #endregion

        #region Private Fields

        private readonly DataStoreProvider _dataStoreProvider;

        #endregion

        #region Methods

        public AllFilesViewModel():base()
        {
            RelevatFilesCollection = AllFiles;
            _dataStoreProvider = new DataStoreProvider();
            LoadSavedFiles();

            DownloadingFilesViewModel = new DownloadingFilesViewModel(AllFiles);
            UploadingFilesViewModel = new UploadingFilesViewModel(AllFiles);
            DownloadedFilesViewModel = new DownloadedFilesViewModel(AllFiles);
        }

        public override void AddFile(MamanNetFile file)
        {
            AllFiles.Add(file);
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
                var mamanNetFile = new MamanNetFile(serializedFile);
                AllFiles.Add(mamanNetFile);
            }
        }

        #endregion

        public override void FilterAllFilesToCollectionFiles()
        {
            throw new NotImplementedException();
        }
    }
}
