using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Utilities;
using Networking.Network;
using Networking.Utilities;

namespace Networking.Packets
{
    [Serializable]
    public class FilePartsRequestPacket : Packet
    {
        readonly byte[] _fileHash;
        readonly int[] _parts;

        public FilePartsRequestPacket(byte[] fileHash, int[] parts)
        {
            _fileHash = (byte[])fileHash.Clone();
            _parts = (int[])parts.Clone();
        }

        public byte[] FileHash
        {
            get { return (byte[])_fileHash.Clone(); }
        }

        public int[] Parts
        {
            get { return (int[])_parts.Clone(); }
        }

        public override string ToString()
        {
            return string.Format("<FilePartsRequestPacket {0}/{1}>", HexConverter.ByteArrayToHexString(_fileHash), string.Join(",", _parts));
        }
    }
}
