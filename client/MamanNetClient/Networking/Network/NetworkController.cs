using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Common.Utilities;
using Networking.Files;
using Networking.Packets;
using System.Configuration;
using System.Data.SqlTypes;
using System.Threading;

namespace Networking.Network
{
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] left, byte[] right)
        {
            if (left == null || right == null)
            {
                return left == right;
            }
            return left.SequenceEqual(right);
        }
        public int GetHashCode(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            return key.Sum(b => b);
        }
    }

    public class NetworkController
    {
        #region Private Members

        private ObservableCollection<MamaNetFile> _files;
        private UdpClient _client;
        private IPEndPoint _myEndPoint;
        private readonly int _port;
        private Timer _hubTimer;

        #endregion

        #region Ctor
        public NetworkController(ObservableCollection<MamaNetFile> files = null, int? port = null)
        {
            if (files == null)
            {
                files = new ObservableCollection<MamaNetFile>();
            }
            if (!port.HasValue)
            {
                port = int.Parse(ConfigurationManager.AppSettings["DefaultClientPort"]);
            }

            _files = files;
            _port = port.Value;

            _hubTimer = new Timer((state) => SynchronizeHubPeriodically(), null, 0, 5000);
            IsListenning = false;
        }

        #endregion

        #region Public API

        public void AddFile(MamaNetFile file)
        {
            
            _files.Add(file);
        }

        public async void StartListen()
        {
            SetupConnection();
            IsListenning = true;

            while (IsListenning)
            {
                //TODO: handle dispose exception  - cannot access a disposed object
                var recieveResult = await _client.ReceiveAsync();
                Task.Run(() => HandlePacket(recieveResult));
            }
        }

        public void Close()
        {
            _client.Close();
            foreach (var file in _files)
            {
                file.Dispose();
            }
            IsListenning = false;
        }

        public bool IsListenning
        {
            get;
            private set;
        }
        #endregion

        #region Packet Handling

        private void HandlePacket(UdpReceiveResult receiveResult)
        {
            var endPoint = receiveResult.RemoteEndPoint;
            var data = receiveResult.Buffer;

            var formatter = new BinaryFormatter();
            var packet = formatter.Deserialize(new MemoryStream(data)) as Packet;

            Logger.WriteLogEntry(string.Format("Got {0} from {1} to {2}", packet, endPoint, _myEndPoint), LogSeverity.Info);

            var dataPacket = packet as DataPacket;
            if (dataPacket != null)
            {
                HandleDataPacket(dataPacket);
            }
            else if (packet is FilePartsRequestPacket)
            {
                HandleFilePartsRequest((FilePartsRequestPacket)packet, endPoint);
            }
        }

        private void HandleFilePartsRequest(FilePartsRequestPacket packet, IPEndPoint peer)
        {

            var relevantFile = _files.SingleOrDefault(file => file.ExpectedHash.SequenceEqual(packet.FileHash));
            if (relevantFile == null)
                return;
            foreach (var part in packet.Parts)
            {
                if (relevantFile[part].IsPartAvailable)
                {
                    SendPacket(DataPacket.FromFilePart(relevantFile[part]), peer);
                }
            }
        }

        private void HandleDataPacket(DataPacket packet)
        {
            if (!packet.VerifyHash())
            {
                // TODO: Request resend
                return;
            }

            var relevantFile = _files.SingleOrDefault(file => file.ExpectedHash.SequenceEqual(packet.FileHash));
            if (relevantFile == null)
                return;

            if (!relevantFile[packet.PartNumber].IsPartAvailable)
            {
                relevantFile[packet.PartNumber].SetData(packet.Data);
            }
        }

        #endregion

        #region Low-Level Networking
        private void SetupConnection()
        {
            if (_client != null) return;
            _myEndPoint = new IPEndPoint(IPAddress.Any, _port);
            _client = new UdpClient(_myEndPoint);
        }

        public async void SendPacket(Packet packet, IPEndPoint peer)
        {
            Logger.WriteLogEntry(string.Format("Send {0} from {1} to {2}", packet, _myEndPoint, peer), LogSeverity.Info);
            SetupConnection();

            var formatter = new BinaryFormatter();

            byte[] buffer;
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, packet);
                buffer = ms.GetBuffer();
            }
            await _client.SendAsync(buffer, buffer.Length, peer);
        }

        #endregion

        #region Hub Communication
        public void SynchronizeHubPeriodically()
        {
           foreach (var file in _files)
            {
                PeerDetails myDetails = new PeerDetails(_port, file.GetAvailableParts());
                if (!file.IsActive) continue;

                foreach (var hub in file.RelatedHubs)
                {
                    var url = new StringBuilder(hub);
                    if (!hub.EndsWith("/"))
                    {
                        url.Append("/");
                    }
                    url.Append(file.HexHash);
                    Task.Run(()=>GetPeers(url.ToString(), myDetails));
                }
            }
        }

        private async void UpdateFileFromHub(MamaNetFile file, string hubUrl, PeerDetails myDetails)
        {
            var peers = await GetPeers(hubUrl, myDetails);

            file.Peers = peers.ToList();
            int[] missingParts = file.GetMissingParts();

            foreach (var peer in peers)
            {
                SendPacket(new FilePartsRequestPacket(file.ExpectedHash, missingParts), peer.IPEndPoint);
            }
        }

        private async Task<List<PeerDetails>> GetPeers(string hubUrl, PeerDetails myDetails)
        {
            var request = WebRequest.CreateHttp(hubUrl);
            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";

            var stream = await request.GetRequestStreamAsync();
            var serializer = new DataContractJsonSerializer(typeof(PeerDetails));
            serializer.WriteObject(stream, myDetails);
            stream.Close(); // Send the request

            var response = await request.GetResponseAsync();
            var deserializer = new DataContractJsonSerializer(typeof(List<PeerDetails>));
            var responseStream = response.GetResponseStream();
            var result = (List<PeerDetails>)deserializer.ReadObject(responseStream);
            if (responseStream != null) responseStream.Close();

            return result;
        }

        #endregion
    }
}
