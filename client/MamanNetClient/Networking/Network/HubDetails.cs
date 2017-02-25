using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Network
{
    [Serializable]
    public class HubDetails:IComparable<HubDetails>
    {
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
        
        public DateTime LastCommunicationTime { get; set; }
        
        public int ConnectedUsers { get; set; }

        public int CompareTo(HubDetails other)
        {
            if (other.Url == Url)
                return 0;
            return 1;
        }
    }
}
