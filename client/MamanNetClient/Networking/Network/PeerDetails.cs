using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Network
{
    [Serializable]
    public class PeerDetails:INotifyPropertyChanged
    {
        private int port;
        private string ip;
        private int[] availableFileParts;
        private string hostname;

        public string Hostname
        {
            get { return hostname; }
            set { hostname = value; }
        }

        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Port"));   
                }
            }
        }

        public string Ip
        {
            get
            {
                return ip;
            }
            set 
            {
                ip = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Ip"));
                }
            }
        }

        public int[] AvailableFileParts
        {
            get
            {
                return availableFileParts;
            }
            set
            {
                availableFileParts = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AvailableFileParts"));
                }
            }
        }


        public PeerDetails(int port, int[] availableFileParts)
        {
            Port = port;
            AvailableFileParts = availableFileParts;
            Hostname = Dns.GetHostName();
        }

        public IPEndPoint IPEndPoint
        {
            get
            {
                return Ip == null ? null : new IPEndPoint(IPAddress.Parse(Ip), Port);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
