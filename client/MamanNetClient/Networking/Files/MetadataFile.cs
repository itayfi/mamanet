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
        
        private ObservableCollection<HubDetails> _relatedHubsDetails;

        private string[] _relatedHubs;
        private string _fullName;
        private int _size;
        private int _partSize;
        private string _description;

        private string _indexer;

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

        public ObservableCollection<HubDetails> RelatedHubs
        {
            get
            {
                if (_relatedHubsDetails != null)
                {
                    return _relatedHubsDetails;
                }
                return null;
            }
            set
            {
                _relatedHubsDetails = value;
                _relatedHubs = value.Select(hub=>hub.Url).ToArray();
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

        public string Description
        {
            set { _description = value; }
            get { return _description; }
        }

        public string Indexer
        {
            set { _indexer = value; }
            get { return _indexer; }
        }

        #endregion

        #region Methods

        public MetadataFile(string fullName, byte[] expectedHash, ObservableCollection<HubDetails> relatedHubs, int size, int partSize, string description, string indexer)
        {
            FullName = fullName;
            ExpectedHash = expectedHash;
            RelatedHubs = relatedHubs;
            Size = size;
            PartSize = partSize;
            Description = description;
            Indexer = indexer;
        }

        public MetadataFile(MetadataFile other)
        {
            FullName = other.FullName;
            _hash = (byte[])other._hash.Clone();
            RelatedHubs = (other._relatedHubsDetails != null) ? other._relatedHubsDetails : null;
            Size = other.Size;
            PartSize = other.PartSize;
            Description = other.Description;
            Indexer = other.Indexer;
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
                ((_relatedHubsDetails == null || other._relatedHubsDetails == null) ? _relatedHubsDetails == other._relatedHubsDetails : other._relatedHubsDetails.OrderBy(h => h).SequenceEqual(_relatedHubsDetails.OrderBy(h => h)));
        }

        public override int GetHashCode()
        {
            return _fullName.GetHashCode() + _hash.GetHashCode() + (_relatedHubsDetails != null ? _relatedHubsDetails.OrderBy(h => h).ToArray().GetHashCode() : -1);
        }


        #endregion
    }
}
