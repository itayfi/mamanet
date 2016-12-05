using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Models.Files
{
    [Serializable]
    public class MamanetFile
    {
        private string name;
        private byte[] hash;
        private string[] hubs;
        private int fileSize;

        public MamanetFile(string name, byte[] hash, string[] hubs, int fileSize)
        {
            this.name = name;
            this.hash = (byte[])hash.Clone();
            this.hubs = (string[])hubs.Clone();
            this.fileSize = fileSize;
        }

        public void Save(string path)
        {
            using (var stream = File.OpenWrite(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }

        public static MamanetFile Load(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var stream = File.OpenRead(path))
            {
                return (MamanetFile)formatter.Deserialize(stream);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MamanetFile))
            {
                return false;
            }
            MamanetFile other = (MamanetFile)obj;
            return other.name == name && other.hash.SequenceEqual(hash) &&
                other.hubs.OrderBy(h => h).SequenceEqual(hubs.OrderBy(h => h));
        }
    }
}
