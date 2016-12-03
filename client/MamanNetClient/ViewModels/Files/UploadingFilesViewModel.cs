using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ViewModel.Files
{
    public class UploadingFilesViewModel : INotifyPropertyChanged
    {
        #region Public Fields
        public ObservableCollection<MamanNetFile> UploadingFiles { get; set; }

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
        private ObservableCollection<MamanNetFile> _allFiles { get; set; }
        private int _uploadSpeed;
        #endregion

        #region Methods
        public UploadingFilesViewModel(ObservableCollection<MamanNetFile> allFiles)
        {
            _allFiles = allFiles;
            UploadingFiles = new ObservableCollection<MamanNetFile>();
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

        private void AddUploadingFile(MamanNetFile serializedMamanNetFile)
        {
            UploadingFiles.Add(serializedMamanNetFile);
            _allFiles.Add(serializedMamanNetFile);
        }

        private void RemoveUploadingFile(MamanNetFile serializedMamanNetFile)
        {
            UploadingFiles.Remove(serializedMamanNetFile);
            _allFiles.Remove(serializedMamanNetFile);
        }

        #endregion
    }
}
