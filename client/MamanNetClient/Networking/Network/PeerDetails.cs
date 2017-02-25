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
    public class PeerDetails
    {
        private int port;
        private string ip;
        private int[] availableFileParts;

        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
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
            }
        }


        public PeerDetails(int port, int[] availableFileParts, string ip="")
        {
            Port = port;
            AvailableFileParts = availableFileParts;
        }

        public IPEndPoint IPEndPoint
        {
            get
            {
                return Ip == null ? null : new IPEndPoint(IPAddress.Parse(Ip), Port);
            }
        }
    }
}
