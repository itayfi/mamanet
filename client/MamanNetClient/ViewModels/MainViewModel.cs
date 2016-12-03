using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using GalaSoft.MvvmLight.Command;
using ViewModel.FilesViewModels;

namespace ViewModel
{
    public class MainViewModel:INotifyPropertyChanged
    {
        //Private Fields
        private string _name;

        //Public Fields
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<string> ShowPopup;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                FireChangeEvent("Name");
            }
        }

        //Public Methods
        public MainViewModel()
        {
            ExitApplication = new RelayCommand(_exitApplication);
        }

        public RelayCommand ExitApplication { get; set; }
        public RelayCommand<string> SelectedCommandChanged { get; set; }

        //Private Methods

        private void _exitApplication()
        {
            Environment.Exit(5);
        }

        private void _changeName()
        {
            Name = "Ron Wisley";
        }

        private void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
