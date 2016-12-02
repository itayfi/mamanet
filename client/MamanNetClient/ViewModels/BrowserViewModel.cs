using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ViewModel
{
    public class BrowserViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<WebsiteViewModel> MamanWebsites { get; set; }

        public BrowserViewModel()
        {
            MamanWebsites = new ObservableCollection<WebsiteViewModel>();
            var website1 = new MamanWebsite() {WebsiteName = "Ynet", WebsiteUri = new Uri("http://www.ynet.co.il")};
            var website2 = new MamanWebsite() { WebsiteName = "Walla", WebsiteUri = new Uri("http://www.walla.co.il") };
            MamanWebsites.Add(new WebsiteViewModel(website1));
            MamanWebsites.Add(new WebsiteViewModel(website2));
        }

        private void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
