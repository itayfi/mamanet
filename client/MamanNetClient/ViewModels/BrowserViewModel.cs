using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Utils;
using GalaSoft.MvvmLight.Command;

namespace ViewModels
{
    public class BrowserViewModel
    {
        #region Public Fields

        public RelayCommand OpenWebBrowser { get; set; }

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
             var hubEndPoints = (ConfigurationManager.GetSection("availableIndexers") as EndPointsConfigurationSection);
            foreach (EndPointElement hubElement in hubEndPoints.Instance)
            {
                Process.Start(hubElement.EndPoint);
            }
        }

        #endregion
    }
}
