using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.Files;

namespace DAL
{
    [Serializable]
    public class DataStore
    {
        public DataStore()
        {
            SavedDataFiles = new List<SharedFile>();
        }

        public void AddDataFileToDataStore(SharedFile file)
        {
            SavedDataFiles.Add(file);
        }

        public List<SharedFile> SavedDataFiles{ get; set; }
    }
}
