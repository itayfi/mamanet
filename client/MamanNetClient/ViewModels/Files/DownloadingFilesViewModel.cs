using System;
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
            var downloadedingFiles = AllFiles.Where(file => file.FileStatus != FileStatus.Downloaded);
            foreach (var file in downloadedingFiles)
            {
                DownloadingFiles.Add(file);
            }
        }

        #endregion
    }
}
