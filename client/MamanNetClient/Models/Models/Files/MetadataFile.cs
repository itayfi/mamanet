﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Utilities;

namespace Common.Models.Files
{
    [Serializable]
    public class MetadataFile
    {
        protected byte[] hash;
        protected string[] hubs;

        public MetadataFile(string name, byte[] hash, string[] hubs, int fileSize, int partSize)
        {
            this.Name = name;
            this.hash = (byte[])hash.Clone();
            this.hubs = hubs != null ? (string[])hubs.Clone() : null;
            this.FileSize = fileSize;
            this.PartSize = partSize;
        }

        public MetadataFile(MetadataFile other)
        {
            Name = other.Name;
            hash = (byte[])other.hash.Clone();
            hubs = (string[])(other.hubs != null ? other.hubs.Clone() : null);
            FileSize = other.FileSize;
            PartSize = other.PartSize;
        }

        public string Name
        {
            get; set;
        }
        public byte[] Hash
        {
            get { return (byte[])hash.Clone(); }
            set { this.hash = (byte[])value.Clone(); }
        }
        public string HexHash
        {
            get { return Utils.ByteArrayToHexString(hash); }
            set { this.hash = (byte[])value.Clone(); }
        }
        public int FileSize
        {
            get; private set;
        }
        public int PartSize
        {
            get; private set;
        }
        public string[] Hubs
        {
            get { return (string[])hubs.Clone(); }
        }
        public int NumberOfParts
        {
            get
            {
                return Convert.ToInt32(Math.Ceiling(Convert.ToDouble(FileSize) / Convert.ToDouble(PartSize)));
            }
        }

        public void Save(string path)
        {
            using (var stream = File.OpenWrite(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }

        public static MetadataFile Load(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var stream = File.OpenRead(path))
            {
                return (MetadataFile)formatter.Deserialize(stream);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MetadataFile))
            {
                return false;
            }
            MetadataFile other = (MetadataFile)obj;
            return other.Name == Name && other.hash.SequenceEqual(hash) &&
                ((hubs == null || other.hubs == null) ? hubs == other.hubs : other.hubs.OrderBy(h => h).SequenceEqual(hubs.OrderBy(h => h)));
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + hash.GetHashCode() + (hubs != null ? hubs.OrderBy(h => h).ToArray().GetHashCode() : -1);
        }
    }
}
