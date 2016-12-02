using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ViewModel.FilesViewModels
{
    public class BaseFilesViewModel:INotifyPropertyChanged
    {
        //Public Fields
        public ObservableCollection<MamanNetFile> AllFilesCollection { get; set; }

        public DownloadingFilesViewModel DownloadingFilesViewModel { get; set; }
        public UploadingFilesViewModel UploadingFilesViewModel { get; set; }
        public DownloadedFilesViewModel DownloadedFilesViewModel { get; set; }

        public void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public BaseFilesViewModel()
        {
            AllFilesCollection = new ObservableCollection<MamanNetFile>();
            DownloadingFilesViewModel = new DownloadingFilesViewModel(AllFilesCollection);
            UploadingFilesViewModel = new UploadingFilesViewModel(AllFilesCollection);
            DownloadedFilesViewModel = new DownloadedFilesViewModel(AllFilesCollection);

            //var context = new SqLiteContext();
            //context.Connect();
            //context.GetAllSavedFiles();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
