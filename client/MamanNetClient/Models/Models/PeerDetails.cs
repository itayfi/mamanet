using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [Serializable]
    public class PeerDetails : IComparable<PeerDetails>
    {
        public PeerDetails(int port, decimal availability, string ip=null)
        {
            Port = port;
            Availability = availability;
            IP = ip;
        }

        public int Port { get; set; }
        public decimal Availability { get; set; }
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

        public int CompareTo(PeerDetails other)
        {
            return Availability.CompareTo(other.Availability);
        }
    }
}
