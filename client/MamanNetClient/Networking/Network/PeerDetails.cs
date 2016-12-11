using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Network
{
    [Serializable]
    public class PeerDetails
    {
        public PeerDetails(int port, string ip=null)
        {
            Port = port;
            IP = ip;
        }

        public int Port { get; set; }
        public string IP { get; set; }

        public IPEndPoint IPEndPoint
        {
            get
            {
                if (IP == null)
                {
                    return null;
                }
                return new IPEndPoint(IPAddress.Parse(IP), Port);
            }
        }
    }
}
