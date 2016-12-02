using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Models;
using ViewModel.DAL;

namespace ViewModel
{
    public class Files:INotifyPropertyChanged
    {

        //Public Fields
        public ObservableCollection<File> FilesCollection { get; set; }

        private void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Files(string filesType)
        {
            var context = new SqLiteContext();
            context.Connect();
            context.Get();

        }
        public Files()
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
