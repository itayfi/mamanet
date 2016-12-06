using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models.Files;

namespace DAL
{
    [Serializable]
    public class DataStore
    {
        public DataStore()
        {
            SavedDataFiles = new List<MamaNetFile>();
        }

        public void AddDataFileToDataStore(MamaNetFile file)
        {
            SavedDataFiles.Add(file);
        }

        public List<MamaNetFile> SavedDataFiles{ get; set; }
    }
}
