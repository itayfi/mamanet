using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Files;

namespace ViewModel.Files
{
    public class DownloadingFilesViewModel:BaseFilesViewModel
    {
        #region Public Fields
        public ObservableCollection<SharedFile> DownloadingFiles { get; set; }

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
        public DownloadingFilesViewModel(ObservableCollection<SharedFile> allFiles):base(allFiles)
        {
            DownloadingFiles = new ObservableCollection<SharedFile>();
            RelevatFilesCollection = DownloadingFiles;
            DownloadSpeed = 500;
            FilterAllFilesToCollectionFiles();
        }

        public sealed override void FilterAllFilesToCollectionFiles()
        {
            var downloadedingFiles = AllFiles.Where(file => file.DownloadStatus != DownloadStatus.Downloaded);
            foreach (var file in downloadedingFiles)
            {
                DownloadingFiles.Add(file);
            }
        }

        #endregion
    }
}
