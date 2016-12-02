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
        private object _selectedPage;

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

        //public object SelectedPage
        //{
        //    get
        //    {
        //        return _selectedPage;
        //    }
        //    set
        //    {
        //        _selectedPage = value;
        //        FireChangeEvent("SelectedPage");
        //    }
        //}

        //Public Methods
        public MainViewModel()
        {
            //SelectedPage = new HomepageViewModel();
            ChangeName = new RelayCommand(_changeName);
            //SelectedCommandChanged = new RelayCommand<string>(_selectedPageChanged);
            ExitApplication = new RelayCommand(_exitApplication);
        }

        public RelayCommand ExitApplication { get; set; }
        public RelayCommand ChangeName { get; set; }
        public RelayCommand<string> SelectedCommandChanged { get; set; }

        //Private Methods

        private void _exitApplication()
        {
            Environment.Exit(1);
        }

        private void _changeName()
        {
            Name = "Ron Wisley";
        }

        //private void _selectedPageChanged(string pageName)
        //{
        //    switch (pageName.ToLower())
        //    {
        //        case "מסך פתיחה":
        //                SelectedPage=new HomepageViewModel();
        //                break;
        //        case "קבצים":
        //                SelectedPage = new BaseFilesViewModel();
        //                if (ShowPopup != null)
        //                {
        //                    ShowPopup(this, "קובץ ירד בהצלחה!");
        //                }
        //                break;
        //        case "בהורדה כעת":
        //                SelectedPage = new BaseFilesViewModel();
        //                break;
        //        case "בשיתוף כעת":
        //                SelectedPage = new BaseFilesViewModel();
        //                break;
        //        case "מוכנים":
        //                SelectedPage = new BaseFilesViewModel();
        //                break;
        //        case "אתרי חיפוש":
        //                SelectedPage = new WebsiteViewModel();
        //                break;
        //        default:
        //                SelectedPage = new BrowserViewModel();
        //                break;
        //    }
        //}

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
