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
    public class UploadingFilesViewModel : INotifyPropertyChanged
    {
        #region Public Fields
        public ObservableCollection<SharedFile> UploadingFiles { get; set; }

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
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Fields
        private ObservableCollection<SharedFile> _allFiles { get; set; }
        private int _uploadSpeed;
        #endregion

        #region Methods
        public UploadingFilesViewModel(ObservableCollection<SharedFile> allFiles)
        {
            _allFiles = allFiles;
            UploadingFiles = new ObservableCollection<SharedFile>();
            UploadSpeed = 24;
        }
        public void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void AddUploadingFile(SharedFile serializedMamanNetFile)
        {
            UploadingFiles.Add(serializedMamanNetFile);
            _allFiles.Add(serializedMamanNetFile);
        }

        private void RemoveUploadingFile(SharedFile serializedMamanNetFile)
        {
            UploadingFiles.Remove(serializedMamanNetFile);
            _allFiles.Remove(serializedMamanNetFile);
        }

        #endregion
    }
}
