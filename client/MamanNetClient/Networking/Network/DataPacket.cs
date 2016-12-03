using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Models.Files;

namespace Networking.Network
{
    [Serializable]
    class DataPacket : Packet
    {
        private byte[] fileHash;
        private int partNumber;
        private byte[] data;
        private byte[] dataHash;

        public byte[] FileHash
        {
            get
            {
                return fileHash;
            }
        }

        public int PartNumber
        {
            get
            {
                return partNumber;
            }
        }

        public byte[] Data
        {
            get
            {
                return (byte[])data.Clone();
            }
        }

        public DataPacket(byte[] fileHash, int partNumber, byte[] data)
        {
            this.fileHash = (byte[])fileHash.Clone();
            this.partNumber = partNumber;
            this.data = (byte[])data.Clone();
            this.dataHash = CalculateHash(this.data);
        }

        private static byte[] CalculateHash(byte[] input)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(input);
        }

        public bool VerifyHash()
        {
            return dataHash.SequenceEqual(CalculateHash(data));
        }

        public static DataPacket FromFilePart(FilePart part)
        {
            return new DataPacket(part.File.Hash, part.Number, part.GetData());
        }

        public override string ToString()
        {
            return String.Format("<DataPacket {0}/{1}>", Utils.ByteArrayToHexString(fileHash), partNumber);
        }
    }
}
