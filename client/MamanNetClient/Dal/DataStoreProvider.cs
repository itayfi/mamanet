using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Common.Models.Files;

namespace DAL
{
    public class DataStoreProvider
    {
        private IFormatter BinaryFormatter { get; set; }
        private const string FileName = "MamaNetDataStore.bin";


        public DataStoreProvider()
        {
            BinaryFormatter = new BinaryFormatter();
        }

        public DataStore LoadData()
        {
            DataStore data;
            using (var fileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
            {
                if (fileStream.Length > 0)
                {
                    data = (DataStore)BinaryFormatter.Deserialize(fileStream);
                }
                else
                {
                    data = new DataStore();
                }
            }
            return data;
        }

        public void SaveData(List<MamaNetFile> mamanNetFiles)
        {
            var data = new DataStore();

            foreach (var file in mamanNetFiles)
            {
                data.AddDataFileToDataStore(file);
            }

            using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter.Serialize(fileStream, data);
            }
        }
    }
}
