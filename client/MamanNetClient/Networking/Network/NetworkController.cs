﻿using Models.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

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
        public const int DEFAULT_PORT = 20588;
        private Dictionary<byte[], MamaNetFile> files;
        private UdpClient client;
        private IPEndPoint myEndPoint;
        private int port;

        public NetworkController(int port = DEFAULT_PORT)
        {
            this.files = new Dictionary<byte[], MamaNetFile>(new ByteArrayComparer());
            this.port = port;
        }

        #region Public API
        public void AddFile(MamaNetFile file)
        {
            files.Add(file.Hash, file);
        }

        public void StartListen()
        {
            SetupConnection();

            Tuple<UdpClient, IPEndPoint> state = new Tuple<UdpClient, IPEndPoint>(client, myEndPoint);

            client.BeginReceive(new AsyncCallback(HandlePacket), state);
        }

        public void UpdateFromHub()
        {
            foreach (var file in files.Values)
            {

            }
        }

        public void Close()
        {
            client.Close();
            foreach (var file in files.Values)
            {
                file.Close();
            }
        }
        #endregion

        #region Packet Handling

        private void HandlePacket(IAsyncResult ar)
        {
            Tuple<UdpClient, IPEndPoint> state = (Tuple<UdpClient, IPEndPoint>)ar.AsyncState;
            var endPoint = state.Item2;
            byte[] data = state.Item1.EndReceive(ar, ref endPoint);

            BinaryFormatter formatter = new BinaryFormatter();
            Packet packet = formatter.Deserialize(new MemoryStream(data)) as Packet;
            Console.WriteLine("Got {0} from {1} to {2}", packet, endPoint, myEndPoint);


            if (packet is DataPacket)
            {
                HandleDataPacket((DataPacket)packet);
            }
            else if (packet is PartRequestPacket)
            {
                HandlePartRequest((PartRequestPacket)packet, endPoint);
            }

            StartListen();
        }

        private void HandlePartRequest(PartRequestPacket packet, IPEndPoint peer)
        {
            if (files.ContainsKey(packet.FileHash))
            {
                var file = files[packet.FileHash];
                foreach (var part in packet.Parts)
                {
                    if (file[part].IsAvailable)
                    {
                        SendPacket(DataPacket.FromFilePart(file[part]), peer);
                    }
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
            if (!files.ContainsKey(packet.FileHash))
            {
                // TODO: wtf
                return;
            }
            if (!files[packet.FileHash][packet.PartNumber].IsAvailable)
            {
                files[packet.FileHash][packet.PartNumber].SetData(packet.Data);
            }
        }

        #endregion

        #region Low-Level Networking
        private void SetupConnection()
        {
            myEndPoint = new IPEndPoint(IPAddress.Any, port);
            if (client == null)
            {
                client = new UdpClient(myEndPoint);
            }
        }

        private void EndSendPacket(IAsyncResult ar)
        {
            client.EndSend(ar);
        }

        public void SendPacket(Packet packet, IPEndPoint peer)
        {
            Console.WriteLine("Send {0} from {1} to {2}", packet, myEndPoint, peer);
            SetupConnection();

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, packet);
            byte[] buffer = ms.GetBuffer();

            client.BeginSend(buffer, buffer.Length, peer, new AsyncCallback(EndSendPacket), null);
        }
        #endregion

        #region Hub Communication
        #endregion
    }
}