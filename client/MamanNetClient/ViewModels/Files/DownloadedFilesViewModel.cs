using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using GalaSoft.MvvmLight.Command;
using Networking.Files;

namespace ViewModels.Files
{
    public class DownloadedFilesViewModel:BaseFilesViewModel
    {
        #region Public Fields

        public ObservableCollection<MamaNetFile> DownloadedFiles { get; set; }
       
        #endregion

        #region Private Fields
        
        #endregion

        #region Methods

        public DownloadedFilesViewModel(ObservableCollection<MamaNetFile> allFiles):base(allFiles)
        {
            DownloadedFiles = new ObservableCollection<MamaNetFile>();
            RelevatFilesCollection = DownloadedFiles;
            FilterAllFilesToCollectionFiles();
        }

        public override bool _canAddFile(string filePath)
        {
            return false;
        }

        public sealed override void FilterAllFilesToCollectionFiles()
        {
            var downloadedFiles = AllFiles.Where(file => file.Availability == 1);
            foreach (var file in downloadedFiles)
            {
                DownloadedFiles.Add(file);
            }
        }

        #endregion
    }
}
