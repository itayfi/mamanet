using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAL
{
    [Serializable]
    public class DataStore
    {
        public DataStore()
        {
            SavedDataFiles = new List<ISerializedMamanNetFile>();
        }

        public void AddDataFileToDataStore(ISerializedMamanNetFile file)
        {
            SavedDataFiles.Add(file);
        }

        public List<ISerializedMamanNetFile> SavedDataFiles{ get; set; }
    }
}
