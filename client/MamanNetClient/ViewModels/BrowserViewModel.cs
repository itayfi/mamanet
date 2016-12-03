﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Models;

namespace ViewModel
{
    public class BrowserViewModel:INotifyPropertyChanged
    {
        #region Public Fields

        public RelayCommand OpenWebBrowser { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Fields

        #endregion

        #region Methods
        public BrowserViewModel()
        {
            OpenWebBrowser = new RelayCommand(_OpenWebBrowser);
        }
        private void _OpenWebBrowser()
        {
            Process.Start("http://www.ynet.co.il");
        }

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
