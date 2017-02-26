using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Network
{
    [Serializable]
    public class HubDetails:IComparable<HubDetails>, INotifyPropertyChanged
    {
        private DateTime _lastCommunicaitonTime;
        private int _connectedUsers;

        public HubDetails(string url)
        {
            Url = url;
            ConnectedUsers = 0;
            LastCommunicationTime = DateTime.Now;
        }

        public HubDetails(string url, int connectedUsers)
        {
            Url = url;
            ConnectedUsers = connectedUsers;
            LastCommunicationTime = DateTime.Now;
        }

        public string Url { get; set; }

        public DateTime LastCommunicationTime
        {
            get { return _lastCommunicaitonTime; }
            set
            {
                _lastCommunicaitonTime = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LastCommunicationTime"));
                }
            }
        }

        public int ConnectedUsers
        {
            get { return _connectedUsers; }
            set
            {
                _connectedUsers = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ConnectedUsers"));
                }
            }
        }

        public int CompareTo(HubDetails other)
        {
            if (other.Url == Url)
                return 0;
            return 1;
        }

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
