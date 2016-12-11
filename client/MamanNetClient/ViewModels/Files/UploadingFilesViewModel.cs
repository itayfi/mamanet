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
    public class UploadingFilesViewModel : BaseFilesViewModel
    {
        #region Public Fields
        public ObservableCollection<MamaNetFile> UploadingFiles { get; set; }

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

        #endregion

        #region Private Fields

        private int _uploadSpeed;
        #endregion

        #region Methods
        public UploadingFilesViewModel(ObservableCollection<MamaNetFile> allFiles):base(allFiles)
        {
            UploadingFiles = new ObservableCollection<MamaNetFile>();
            RelevatFilesCollection = UploadingFiles;
            UploadSpeed = 27;
        }

        public override bool _canAddFile(string filePath)
        {
            return false;
        }
  
        #endregion

        public override void FilterAllFilesToCollectionFiles()
        {
            throw new NotImplementedException();
        }
    }
}
