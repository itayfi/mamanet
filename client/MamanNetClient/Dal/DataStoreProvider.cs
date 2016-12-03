using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Models;

namespace DAL
{
    public class DataStoreProvider
    {
        private IFormatter BinaryFormatter { get; set; }
        private Stream FileStream { get; set; }
        public DataStoreProvider()
        {
            BinaryFormatter = new BinaryFormatter();
            FileStream = new FileStream("MamanNetDataStore.bin", FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None);
        }

        public DataStore LoadData()
        {
            if (FileStream.Length <= 0) return new DataStore();
            DataStore data = (DataStore)BinaryFormatter.Deserialize(FileStream);
            return data;
        }

        public void SaveData(List<MamanNetFile> mamanNetFiles)
        {
            DataStore data = new DataStore();
            var serializeableFiles = mamanNetFiles.ToList<ISerializedMamanNetFile>();

            foreach (var file in serializeableFiles)
            {
                data.AddDataFileToDataStore(file);
            }

            BinaryFormatter.Serialize(FileStream,data);
        }
    }
}
