using Networking.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class MetadataFileProvider
    {
        BinaryFormatter formatter;

        public MetadataFileProvider()
        {
            formatter = new BinaryFormatter();
        }

        public void Save(MetadataFile data, string path)
        {
            using (var stream = File.OpenWrite(path))
            {
                formatter.Serialize(stream, data);
            }
        }

        public MetadataFile Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return (MetadataFile)formatter.Deserialize(stream);
            }
        }
    }
}
