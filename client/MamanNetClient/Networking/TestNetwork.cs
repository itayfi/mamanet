using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Net;
using System.Threading;
using Models.Files;
using Networking.Network;

namespace TestMamanetLib
{
    [TestClass]
    public class TestNetwork
    {
        const string LOREM = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam enim ipsum, aliquam sed massa dignissim, commodo tempor lectus. Nam fringilla condimentum est, at feugiat magna cursus et. In ullamcorper felis vel nisi imperdiet ullamcorper. Fusce sagittis metus eget dolor porta faucibus. Aliquam scelerisque, libero in lobortis lobortis, metus dui commodo risus, ut elementum turpis odio eu leo. Maecenas imperdiet, ipsum ut mattis luctus, mi erat sodales augue, vel vestibulum ipsum justo nec dui. Duis sed erat finibus, commodo nisi et, dapibus nulla. Duis nec ligula sit amet velit vehicula volutpat. Donec ultrices tortor magna, sed accumsan lectus egestas id. Sed tempus finibus ullamcorper. Donec tellus enim, mollis eget felis et, semper cursus purus. Pellentesque convallis semper fermentum. Cras a leo non urna suscipit malesuada. Aliquam vel faucibus nunc. Sed suscipit sollicitudin lectus.\nCum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.Vestibulum in commodo tellus. Ut non dictum nisl, ut scelerisque dui. Maecenas purus nisi, malesuada at varius in, posuere non enim.Vivamus non volutpat quam, non commodo erat. Praesent cursus viverra rhoncus. Aenean congue pellentesque facilisis. Praesent vel nisl varius, posuere quam non, suscipit turpis.Pellentesque efficitur vehicula metus eu rutrum. Etiam vel mi elementum, facilisis felis finibus, semper tortor.Donec est tellus, posuere sed fringilla vel, commodo at turpis. Integer ultrices sem metus, id sagittis dui posuere id. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae;\n            Maecenas massa quam, vulputate sit amet massa in, tincidunt iaculis dolor. Aenean lacinia aliquet purus, a tincidunt justo ultrices a. Mauris ut mauris vel lectus cursus pulvinar.Nunc a purus ac sapien fermentum iaculis.Praesent vulputate et mi iaculis semper. Etiam mollis tortor a est porttitor, vitae tincidunt mauris faucibus.Proin nec ultrices nisl. Duis iaculis posuere diam, id euismod ipsum congue et. Nulla turpi";
        readonly byte[] HASH = new byte[] { 1, 2, 3, 4, 5, 6 };
        readonly byte[] DATA = Encoding.ASCII.GetBytes(LOREM);

        [TestMethod]
        [Timeout(5000)]
        public void TestBasicSend()
        {
            string sourceFile = Path.GetTempFileName();
            string destFile = Path.GetTempFileName();
            File.WriteAllText(sourceFile, LOREM);
            NetworkController sender = new NetworkController(NetworkController.DEFAULT_PORT + 1);
            NetworkController receiver = new NetworkController(NetworkController.DEFAULT_PORT + 2);
            SharedFile source = new SharedFile("test.txt", HASH, new FileInfo(sourceFile), DATA.Length, isAvailable: true);
            SharedFile dest = new SharedFile("test.txt", HASH, new FileInfo(destFile), DATA.Length);

            sender.AddFile(source);
            receiver.AddFile(dest);

            sender.StartListen();
            receiver.StartListen();
            receiver.SendPacket(new PartRequestPacket(HASH, new int[] { 0, 1 }), new IPEndPoint(IPAddress.Parse("127.0.0.1"), NetworkController.DEFAULT_PORT + 1));

            while (dest.Availability < 1)
            {
                Thread.Sleep(100);
            }

            sender.Close();
            receiver.Close();

            Assert.AreEqual(File.ReadAllText(sourceFile), File.ReadAllText(destFile));
        }
    }
}
