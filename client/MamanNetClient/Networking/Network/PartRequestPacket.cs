using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Utilities;

namespace Networking.Network
{
    [Serializable]
    public class PartRequestPacket : Packet
    {
        byte[] fileHash;
        int[] parts;

        public PartRequestPacket(byte[] fileHash, int[] parts)
        {
            this.fileHash = (byte[])fileHash.Clone();
            this.parts = (int[])parts.Clone();
        }

        public byte[] FileHash
        {
            get { return (byte[])fileHash.Clone(); }
        }

        public int[] Parts
        {
            get { return (int[])parts.Clone(); }
        }

        public override string ToString()
        {
            return String.Format("<PartRequestPacket {0}/{1}>", Utils.ByteArrayToHexString(fileHash), String.Join(",", parts));
        }
    }
}
