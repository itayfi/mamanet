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
        private int _port;
        private string _ip;
        private int[] _availableFileParts;

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
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
    }
}
