﻿using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using System.Linq;
using Networking.Files;
using DAL;
using Networking.Network;
using Networking.Utilities;

namespace TestMamaNetClient
{
    [TestClass]
    public class TestSharedFile
    {

        const string LOREM = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam enim ipsum, aliquam sed massa dignissim, commodo tempor lectus. Nam fringilla condimentum est, at feugiat magna cursus et. In ullamcorper felis vel nisi imperdiet ullamcorper. Fusce sagittis metus eget dolor porta faucibus. Aliquam scelerisque, libero in lobortis lobortis, metus dui commodo risus, ut elementum turpis odio eu leo. Maecenas imperdiet, ipsum ut mattis luctus, mi erat sodales augue, vel vestibulum ipsum justo nec dui. Duis sed erat finibus, commodo nisi et, dapibus nulla. Duis nec ligula sit amet velit vehicula volutpat. Donec ultrices tortor magna, sed accumsan lectus egestas id. Sed tempus finibus ullamcorper. Donec tellus enim, mollis eget felis et, semper cursus purus. Pellentesque convallis semper fermentum. Cras a leo non urna suscipit malesuada. Aliquam vel faucibus nunc. Sed suscipit sollicitudin lectus.\nCum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.Vestibulum in commodo tellus. Ut non dictum nisl, ut scelerisque dui. Maecenas purus nisi, malesuada at varius in, posuere non enim.Vivamus non volutpat quam, non commodo erat. Praesent cursus viverra rhoncus. Aenean congue pellentesque facilisis. Praesent vel nisl varius, posuere quam non, suscipit turpis.Pellentesque efficitur vehicula metus eu rutrum. Etiam vel mi elementum, facilisis felis finibus, semper tortor.Donec est tellus, posuere sed fringilla vel, commodo at turpis. Integer ultrices sem metus, id sagittis dui posuere id. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae;\n            Maecenas massa quam, vulputate sit amet massa in, tincidunt iaculis dolor. Aenean lacinia aliquet purus, a tincidunt justo ultrices a. Mauris ut mauris vel lectus cursus pulvinar.Nunc a purus ac sapien fermentum iaculis.Praesent vulputate et mi iaculis semper. Etiam mollis tortor a est porttitor, vitae tincidunt mauris faucibus.Proin nec ultrices nisl. Duis iaculis posuere diam, id euismod ipsum congue et. Nulla turpi";
        readonly byte[] DATA = Encoding.ASCII.GetBytes(LOREM);

        [TestMethod]
        public void TestWrite()
        {
            string filename = Path.GetTempFileName();
            using (var file = new MamaNetFile("test", HashUtils.HexStringToByteArray("deadbeef"), filename, 2048,new string[] {"MyHub"}))
            {
                file[0].SetData(DATA.Take(1024).ToArray());
                file[1].SetData(DATA.Skip(1024).ToArray());
            }
            Assert.AreEqual(LOREM, File.ReadAllText(filename));
        }

        [TestMethod]
        public void TestRead()
        {
            string filename = Path.GetTempFileName();
            File.WriteAllText(filename, LOREM);
            using (var file = new MamaNetFile("test", HashUtils.HexStringToByteArray("deadbeef"), filename, 2048, isFullAvailable: true, relatedHubs: new string[] {"MyHub"}))
            {
                var data1 = file[0].GetData();
                var data2 = file[1].GetData();
                var output = Encoding.ASCII.GetString(data1.Concat(data2).ToArray());
                Assert.AreEqual(LOREM, output);
            }
        }

        [TestMethod]
        public void TestReadWrite()
        {
            string filename = Path.GetTempFileName();
            File.WriteAllText(filename, LOREM);
            using (var file = new MamaNetFile("test", HashUtils.HexStringToByteArray("deadbeef"), filename, 2048, isFullAvailable: true, relatedHubs: new string[] { "MyHub" }))
            {
                var data1 = file[0].GetData();
                file[1].SetData(data1);
            }
        }

        [TestMethod]
        public void TestReadNotExactSize()
        {
            string filename = Path.GetTempFileName();
            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            byte[] data = Encoding.ASCII.GetBytes(text);
            File.WriteAllText(filename, text);

            using (var file = new MamaNetFile("test", HashUtils.HexStringToByteArray("deadbeef"), filename, (int)new FileInfo(filename).Length, isFullAvailable: true, relatedHubs: new string[] { "MyHub" }))
            {
                var data1 = file[0].GetData();
                CollectionAssert.AreEqual(data, data1);
            }
        }

        [TestMethod]
        public void TestWriteNotExactSize()
        {
            string filename = Path.GetTempFileName();
            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            byte[] data = Encoding.ASCII.GetBytes(text);
            File.WriteAllText(filename, text);

            using (var file = new MamaNetFile("test", HashUtils.HexStringToByteArray("deadbeef"), filename, data.Length, relatedHubs: new string[] { "MyHub" }))
            {

                file[0].SetData(data);
                Assert.AreEqual(text, File.ReadAllText(filename));
            }
        }

        [TestMethod]
        public void TestMetadataFile()
        {
            string filename = Path.GetTempFileName();
            MetadataFile file = new MetadataFile(new MamaNetFile("test.txt", new byte[] { 1, 2, 3, 4, 5, 6 }, "", 2048, relatedHubs: new string[] { "MyHub" }));
            MetadataFileProvider provider = new MetadataFileProvider();

            provider.SaveAndSend(file, filename);
            MetadataFile file2 = provider.Load(filename);

            Assert.AreEqual(file, file2);
            Assert.IsNotInstanceOfType(file2, typeof(MamaNetFile));
        }
    }
}
