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

        public async void SaveAndSend(MetadataFile data, string path)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                formatter.WriteObject(fileStream, data);
            }

            var request = WebRequest.CreateHttp(data.Indexer+"/upload");
            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";

            var networkStream = await request.GetRequestStreamAsync();
            formatter.WriteObject(networkStream, data);
            networkStream.Close(); // Send the request
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
