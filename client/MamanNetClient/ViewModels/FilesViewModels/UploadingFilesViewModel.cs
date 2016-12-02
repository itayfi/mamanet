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

        public UploadingFilesViewModel(ObservableCollection<MamanNetFile> allFiles)
        {
            _allFiles = allFiles;
            UploadingFiles = new ObservableCollection<MamanNetFile>();
        }

        private void AddDownloadedFile(MamanNetFile file)
        {
            UploadingFiles.Add(file);
            _allFiles.Add(file);
        }

        private void DeleteDownloadedFile(MamanNetFile file)
        {
            UploadingFiles.Remove(file);
            _allFiles.Remove(file);
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
