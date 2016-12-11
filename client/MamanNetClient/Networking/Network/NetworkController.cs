using System;
using System.Collections.Generic;
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

        public static readonly int DefaultPort = int.Parse(ConfigurationManager.AppSettings["DefaultPort"]);
        private readonly Dictionary<byte[], MamaNetFile> _files;
        private UdpClient _client;
        private IPEndPoint _myEndPoint;
        private readonly int _port;

        #endregion

        #region Ctor
        public NetworkController(int port)
        {
            _files = new Dictionary<byte[], MamaNetFile>(new ByteArrayComparer());
            _port = port;
        }

        public NetworkController()
        {
            _files = new Dictionary<byte[], MamaNetFile>(new ByteArrayComparer());
            _port = DefaultPort;
        }

        #endregion

        #region Public API

        public void AddFile(MamaNetFile file)
        {
            _files.Add(file.ExpectedHash, file);
        }

        public void StartListen()
        {
            SetupConnection();

            var state = new Tuple<UdpClient, IPEndPoint>(_client, _myEndPoint);

            //TODO: change BeginRecive to ReceiveAsync
            _client.BeginReceive(HandlePacket, state);
            IsListenning = true;
        }

        public void Close()
        {
            _client.Close();
            foreach (var file in _files.Values)
            {
                file.Close();
            }
            IsListenning = false;
        }

        public bool IsListenning
        {
            get; private set;
        }
        #endregion

        #region Packet Handling

        private void HandlePacket(IAsyncResult ar)
        {
            var state = (Tuple<UdpClient, IPEndPoint>)ar.AsyncState;
            var endPoint = state.Item2;
            var data = state.Item1.EndReceive(ar, ref endPoint);

            var formatter = new BinaryFormatter();
            var packet = formatter.Deserialize(new MemoryStream(data)) as Packet;
            Logger.WriteLogEntry(string.Format("Got {0} from {1} to {2}", packet, endPoint, _myEndPoint),LogSeverity.Info);

            var dataPacket = packet as DataPacket;
            if (dataPacket != null)
            {
                HandleDataPacket(dataPacket);
            }
            else if (packet is FilePartsRequestPacket)
            {
                HandleFilePartsRequest((FilePartsRequestPacket)packet, endPoint);
            }

            StartListen();
        }

        private void HandleFilePartsRequest(FilePartsRequestPacket packet, IPEndPoint peer)
        {
            if (_files.ContainsKey(packet.FileHash))
            {
                //Todo: generate error
                return;
            }

            var file = _files[packet.FileHash];
            foreach (var part in packet.Parts)
            {
                if (file[part].IsPartAvailable)
                {
                    SendPacket(DataPacket.FromFilePart(file[part]), peer);
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
            if (!_files.ContainsKey(packet.FileHash))
            {
                // TODO: wtf
                return;
            }
            if (!_files[packet.FileHash][packet.PartNumber].IsPartAvailable)
            {
                _files[packet.FileHash][packet.PartNumber].SetData(packet.Data);
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

        private void EndSendPacket(IAsyncResult ar)
        {
            _client.EndSend(ar);
        }

        public void SendPacket(Packet packet, IPEndPoint peer)
        {
            Logger.WriteLogEntry(string.Format("Send {0} from {1} to {2}", packet, _myEndPoint, peer),LogSeverity.Info);
            SetupConnection();

            var formatter = new BinaryFormatter();
            var ms = new MemoryStream();
            formatter.Serialize(ms, packet);
            var buffer = ms.GetBuffer();

            _client.BeginSend(buffer, buffer.Length, peer, EndSendPacket, null);
        } 

        #endregion

        #region Hub Communication
        public Task UpdateFromHub()
        {
            List<Task> tasks = new List<Task>();
            foreach (var file in _files.Values)
            {
                PeerDetails myDetails = new PeerDetails(_port);
                if (file.IsActive)
                {
                    foreach (var hub in file.RelatedHubs)
                    {
                        StringBuilder url = new StringBuilder(hub);
                        if (!hub.EndsWith("/"))
                        {
                            url.Append("/");
                        }
                        url.Append(file.HexHash);
                        tasks.Add(GetPeers(url.ToString(), myDetails));
                    }
                }
            }
            return Task.WhenAll(tasks);
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
