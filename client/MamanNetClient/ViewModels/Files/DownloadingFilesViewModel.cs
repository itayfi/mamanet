﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking.Files;

namespace ViewModels.Files
{
    public class DownloadingFilesViewModel:BaseFilesViewModel
    {
        #region Public Fields
        public ObservableCollection<MamaNetFile> DownloadingFiles { get; set; }

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
       
        #endregion

        #region Private Fields
        private int _downloadSpeed;

        #endregion

        #region Methods
        public DownloadingFilesViewModel(ObservableCollection<MamaNetFile> allFiles):base(allFiles)
        {
            DownloadingFiles = new ObservableCollection<MamaNetFile>();
            RelevatFilesCollection = DownloadingFiles;
            DownloadSpeed = 500;
            FilterAllFilesToCollectionFiles();
        }

        public sealed override void FilterAllFilesToCollectionFiles()
        {
            var downloadingFiles = AllFiles.Where(file => file.Availability != 1);
            DownloadingFiles.Clear();
            foreach (var file in downloadingFiles)
            {
                DownloadingFiles.Add(file);
            }
        }

        #endregion
    }
}
