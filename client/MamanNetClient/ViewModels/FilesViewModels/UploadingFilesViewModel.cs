using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ViewModel.FilesViewModels
{
    public class UploadingFilesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MamanNetFile> UploadingFiles { get; set; }
        private ObservableCollection<MamanNetFile> _allFiles { get; set; }
        private int _uploadSpeed;

        public UploadingFilesViewModel(ObservableCollection<MamanNetFile> allFiles)
        {
            _allFiles = allFiles;
            UploadingFiles = new ObservableCollection<MamanNetFile>();
            UploadSpeed = 24;
        }

        private void AddDownloadedFile(MamanNetFile serializedMamanNetFile)
        {
            UploadingFiles.Add(serializedMamanNetFile);
            _allFiles.Add(serializedMamanNetFile);
        }

        private void DeleteDownloadedFile(MamanNetFile serializedMamanNetFile)
        {
            UploadingFiles.Remove(serializedMamanNetFile);
            _allFiles.Remove(serializedMamanNetFile);
        }

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
        public void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
