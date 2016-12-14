using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Networking.Files;
using Networking.Network;
using Networking.Utilities;

namespace Networking.Packets
{
    [Serializable]
    class DataPacket : Packet
    {
        #region Private Members

        private readonly byte[] _fileHash;
        private readonly int _partNumber;
        private readonly byte[] _data;
        private readonly byte[] _dataHash;

        #endregion

        #region Public Members

        public byte[] FileHash
        {
            get
            {
                return _fileHash;
            }
        }

        public int PartNumber
        {
            get
            {
                return _partNumber;
            }
        }

        public byte[] Data
        {
            get
            {
                return (byte[])_data.Clone();
            }
        }

        #endregion

        #region Methods

        public DataPacket(byte[] fileHash, int partNumber, byte[] data)
        {
            _fileHash = (byte[])fileHash.Clone();
            _partNumber = partNumber;
            _data = (byte[])data.Clone();
            _dataHash = CalculateHash(_data);
        }
        public static DataPacket FromFilePart(FilePart part)
        {
            return new DataPacket(part.MamaNetFile.ExpectedHash, part.PartNumber, part.GetData());
        }

        private byte[] CalculateHash(byte[] input)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(input);
        }

        public bool VerifyHash()
        {
            return _dataHash.SequenceEqual(CalculateHash(_data));
        }

        public override string ToString()
        {
            return string.Format("<DataPacket {0}/{1}>", HashUtils.ByteArrayToHexString(_fileHash), _partNumber);
        }

        #endregion
    }
}
