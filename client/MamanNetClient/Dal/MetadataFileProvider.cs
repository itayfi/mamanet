using Networking.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Networking.Network;

namespace DAL
{
    public class MetadataFileProvider
    {
        DataContractJsonSerializer formatter; 


        public MetadataFileProvider()
        {
            formatter = new DataContractJsonSerializer(typeof (MetadataFile));
        }

        public void SaveToFile(MetadataFile data, string path)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                formatter.WriteObject(fileStream, data);
            }
        }

        public async Task UploadToIndexer(MetadataFile data, string indexerUploadUrl)
        { 
            var request = WebRequest.CreateHttp(indexerUploadUrl + "/upload");
            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";

            using (var networkStream = await request.GetRequestStreamAsync())
            {
                 formatter.WriteObject(networkStream, data);
            }

            var response = (await request.GetResponseAsync()) as HttpWebResponse;
            if (response?.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(String.Format("The indexer {0} did not approve your file", indexerUploadUrl));
            }
        }


        public MetadataFile Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return (MetadataFile)formatter.ReadObject(stream);
            }
        }
    }
}
