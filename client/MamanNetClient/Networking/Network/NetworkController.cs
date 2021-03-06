﻿using System;
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
using Networking.Files;
using Networking.Packets;
using System.Configuration;
using System.Data.SqlTypes;
using System.Threading;
using Common.LogUtilities;
using ViewModels.Files;

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
        private readonly string _ip;
        private Timer _hubTimer;
        private TaskScheduler _syncContextScheduler;
        private const int MaxBatchSize = 1024;
        private INotifyFileChange _fileChange;

        #endregion

        #region Ctor
        public NetworkController(ObservableCollection<MamaNetFile> files = null, int? port = null, INotifyFileChange fileChange = null)
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

            if (SynchronizationContext.Current != null)
            {
                _syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            else
            {
                // If there is no SyncContext for this thread (e.g. we are in a unit test
                // or console scenario instead of running in an app), then just use the
                // default scheduler because there is no UI thread to sync with.
                _syncContextScheduler = TaskScheduler.Current;
            }

            _fileChange = fileChange;
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
                #pragma warning disable 4014
                Task.Run(() => HandlePacket(recieveResult));
                #pragma warning restore 4014
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
            var packetsSent = 0;
            foreach (var part in packet.Parts)
            {
                if (relevantFile[part].IsPartAvailable)
                {
                    if (packetsSent > MaxBatchSize)
                    {
                        return;
                    }
                    SendPacket(DataPacket.FromFilePart(relevantFile[part]), peer);
                    packetsSent++;
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

            if (relevantFile.Availability == 1)
            {
                if (_fileChange != null)
                {
                    _fileChange.NotifyFileChange(_syncContextScheduler);
                }
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
            foreach (var mamaNetFile in _files)
            {
                var myFile = mamaNetFile;
                PeerDetails myFileDetails = new PeerDetails(_port, mamaNetFile.GetAvailableParts());
                if (!myFile.IsActive) continue;

                foreach (var hub in mamaNetFile.RelatedHubs)
                {
                    var url = new StringBuilder(hub.Url);
                    url.Append(@"/"+mamaNetFile.HexHash);
                    Task.Run(() => UpdateFileFromHub(myFile, url.ToString(), myFileDetails));
                }
            }
        }

        private async void UpdateFileFromHub(MamaNetFile file, string hubUrl, PeerDetails myDetails)
        {
            var filePeers = Enumerable.Empty<PeerDetails>().ToList();
            //Todo: handle exceptions from Hub
            try
            {
                filePeers = await GetPeers(hubUrl, myDetails);
            }
            catch (Exception e)
            {
                Logger.WriteLogEntry("hub error: " + e.Message, LogSeverity.Error);
                return;
            }
            
            file.SyncPeersInformation(filePeers, _syncContextScheduler);
            var concatedHub = hubUrl.LastIndexOf("/", StringComparison.Ordinal);
            HubDetails hubDetails = new HubDetails(hubUrl.Substring(0, concatedHub), filePeers.Count);
            file.SyncHubInformation(hubDetails, _syncContextScheduler);

            var missingParts = file.GetMissingParts();

            //Meaning that I already have the file
            if (missingParts.Length == 0)
            {
                return;
            }
            
            foreach (var peer in filePeers)
            {
                //intersect filter - which parts I don't have
                var partsToAsk = missingParts.Where(part => peer.AvailableFileParts.Contains(part)).Take(MaxBatchSize).ToArray();
                if (partsToAsk.Any())
                {
                    SendPacket(new FilePartsRequestPacket(file.ExpectedHash, partsToAsk), peer.IPEndPoint);
                }
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
