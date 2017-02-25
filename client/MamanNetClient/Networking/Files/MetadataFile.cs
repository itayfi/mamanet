using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Networking.Network;
using Networking.Utilities;

namespace Networking.Files
{
    [Serializable]
    public class MetadataFile : INotifyPropertyChanged
    {
        #region Private Members

        private byte[] _hash;
        private string[] _relatedHubs;
        private string _fullName;
        private int _size;
        private int _partSize;
        private string _description;

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
            get { return _relatedHubs; }
            set { _relatedHubs = value; }
        }
      
        public int NumberOfParts
        {
            get
            {
                return Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Size) / Convert.ToDouble(PartSize)));
            }
        }

        public string Description
        {
            set { _description = value; }
            get { return _description; }
        }

        #endregion

        #region Methods

        public MetadataFile(string fullName, byte[] expectedHash, string[] relatedHubs, int size, int partSize, string description)
        {
            FullName = fullName;
            ExpectedHash = expectedHash;
            Size = size;
            PartSize = partSize;
            Description = description;
            _relatedHubs = relatedHubs;
        }

        public MetadataFile(MetadataFile other)
        {
            FullName = other.FullName;
            _hash = (byte[])other._hash.Clone();
            Size = other.Size;
            PartSize = other.PartSize;
            Description = other.Description;
            RelatedHubs = other.RelatedHubs;
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
            return other._fullName == _fullName && other._hash.SequenceEqual(_hash);
        }

        public override int GetHashCode()
        {
            return _fullName.GetHashCode() + _hash.GetHashCode();
        }


        #endregion
    }
}
