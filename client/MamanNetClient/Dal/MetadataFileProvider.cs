using Networking.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

        public async Task SaveAndSend(MetadataFile data, string path)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                formatter.WriteObject(fileStream, data);
            }
        }

        public async Task Send(MetadataFile data, string indexer)
        {
            var request = WebRequest.CreateHttp(indexer + "/upload");
            request.Timeout = 500;
            request.ContinueTimeout = 1;
            request.ReadWriteTimeout = 500;
            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";

            using (var networkStream = await request.GetRequestStreamAsync())
            {
                formatter.WriteObject(networkStream, data);
            }

            var response = await request.GetResponseAsync() as HttpWebResponse;
            if (response != null && response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(string.Format("Indexer {0} couldn't get the upload request of file {1}", data.FullName, indexer));
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
