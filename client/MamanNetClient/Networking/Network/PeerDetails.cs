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
        private int _port;
        private string _ip;
        private int[] _availableFileParts;
        private string _hostname;

        public string hostname
        {
            get { return _hostname; }
            set { _hostname = value; }
        }

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
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
                return _ip;
            }
            set 
            {
                _ip = value;
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
                return _availableFileParts;
            }
            set
            {
                _availableFileParts = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AvailableFileParts"));
                }
            }
        }


        public PeerDetails(int port, int[] availableFileParts, string ip = null)
        {
            Port = port;
            Ip = ip;
            AvailableFileParts = availableFileParts;
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
