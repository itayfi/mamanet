using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Utilities;
using Networking.Network;
using Networking.Utilities;

namespace Networking.Files
{
    [Serializable]
    public class MetadataFile : INotifyPropertyChanged
    {
        #region Private Members

        private byte[] _hash;

        //for future support multiple RelatedHubs downloading
        private string[] _relatedHubs;

        private string _fullName;
        private int _size;
        private int _partSize;

        #endregion 

        #region Public Members

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                FireChangeEvent("FullName");
            }
        }

        public byte[] ExpectedHash
        {
            get
            {
                return (byte[])_hash.Clone();
            }
            private set
            {
                _hash = value;
            }
        }

        public string HexHash
        {
            get
            {
                return HashUtils.ByteArrayToHexString(_hash);
            }
        }

        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                FireChangeEvent("Size");
            }
        }

        public int PartSize
        {
            get { return _partSize; }
            set
            {
                _partSize = value;
                FireChangeEvent("PartSize");
            }
        }

        public string[] RelatedHubs
        {
            get
            {
                if (_relatedHubs != null)
                {
                    return (string[])_relatedHubs.Clone();
                }
                return null;
            }
            set
            {
                _relatedHubs = value;
                FireChangeEvent("RelatedHubs");
            }
        }

        public int NumberOfParts
        {
            get
            {
                return Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Size) / Convert.ToDouble(PartSize)));
            }
        }

        #endregion

        #region Methods

        public MetadataFile(string fullName, byte[] expectedHash, string[] relatedHubs, int size, int partSize)
        {
            FullName = fullName;
            ExpectedHash = expectedHash;
            RelatedHubs = relatedHubs ?? new [] {ConfigurationManager.AppSettings["DefaultHubUrl"]};
            Size = size;
            PartSize = partSize;
        }

        public MetadataFile(MetadataFile other)
        {
            FullName = other.FullName;
            _hash = (byte[])other._hash.Clone();
            _relatedHubs = (string[])(other._relatedHubs != null ? other._relatedHubs.Clone() : null);
            Size = other.Size;
            PartSize = other.PartSize;
        }

        protected void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MetadataFile))
            {
                return false;
            }
            MetadataFile other = (MetadataFile)obj;
            return other._fullName == _fullName && other._hash.SequenceEqual(_hash) &&
                ((_relatedHubs == null || other._relatedHubs == null) ? _relatedHubs == other._relatedHubs : other._relatedHubs.OrderBy(h => h).SequenceEqual(_relatedHubs.OrderBy(h => h)));
        }

        public override int GetHashCode()
        {
            return _fullName.GetHashCode() + _hash.GetHashCode() + (_relatedHubs != null ? _relatedHubs.OrderBy(h => h).ToArray().GetHashCode() : -1);
        }


        #endregion
    }
}
