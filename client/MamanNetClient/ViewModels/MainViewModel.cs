using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace ViewModels
{
    public class MainViewModel:INotifyPropertyChanged
    {
        #region Public Fields

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<string> ShowPopup;

        #endregion

        #region Public Methods

        public MainViewModel()
        {
            if (ShowPopup != null)
            {
                ShowPopup(this, "hello world");
            }
        }

        #endregion

        #region Private Methods
        private void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
