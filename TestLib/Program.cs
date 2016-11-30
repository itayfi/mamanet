using LibMamanet.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLib
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = Path.GetTempFileName();
            var file = new MamanetFile("test", "deadbeef", filename, 2048);
            var lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam enim ipsum, aliquam sed massa dignissim, commodo tempor lectus. Nam fringilla condimentum est, at feugiat magna cursus et. In ullamcorper felis vel nisi imperdiet ullamcorper. Fusce sagittis metus eget dolor porta faucibus. Aliquam scelerisque, libero in lobortis lobortis, metus dui commodo risus, ut elementum turpis odio eu leo. Maecenas imperdiet, ipsum ut mattis luctus, mi erat sodales augue, vel vestibulum ipsum justo nec dui. Duis sed erat finibus, commodo nisi et, dapibus nulla. Duis nec ligula sit amet velit vehicula volutpat. Donec ultrices tortor magna, sed accumsan lectus egestas id. Sed tempus finibus ullamcorper. Donec tellus enim, mollis eget felis et, semper cursus purus. Pellentesque convallis semper fermentum. Cras a leo non urna suscipit malesuada. Aliquam vel faucibus nunc. Sed suscipit sollicitudin lectus.\nCum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.Vestibulum in commodo tellus. Ut non dictum nisl, ut scelerisque dui. Maecenas purus nisi, malesuada at varius in, posuere non enim.Vivamus non volutpat quam, non commodo erat. Praesent cursus viverra rhoncus. Aenean congue pellentesque facilisis. Praesent vel nisl varius, posuere quam non, suscipit turpis.Pellentesque efficitur vehicula metus eu rutrum. Etiam vel mi elementum, facilisis felis finibus, semper tortor.Donec est tellus, posuere sed fringilla vel, commodo at turpis. Integer ultrices sem metus, id sagittis dui posuere id. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae;\n            Maecenas massa quam, vulputate sit amet massa in, tincidunt iaculis dolor. Aenean lacinia aliquet purus, a tincidunt justo ultrices a. Mauris ut mauris vel lectus cursus pulvinar.Nunc a purus ac sapien fermentum iaculis.Praesent vulputate et mi iaculis semper. Etiam mollis tortor a est porttitor, vitae tincidunt mauris faucibus.Proin nec ultrices nisl. Duis iaculis posuere diam, id euismod ipsum congue et. Nulla turpi";
            var data = ASCIIEncoding.ASCII.GetBytes(lorem);
            TestWrite(filename, file, lorem, data);
            Console.WriteLine("----------------------------------");
            TestRead(file, lorem, data);
            Console.WriteLine("----------------------------------");
            TestReadWrite(file);
            Console.Write("Press enter to exit...");
            Console.ReadLine();
        }

        private static void TestReadWrite(MamanetFile file)
        {
            Console.WriteLine("Trying to read and write at the same time...");
            Console.WriteLine("Trying to read file...");
            var d1 = file[0].GetData();
            Console.WriteLine("Trying to write to file file...");
            file[1].SetData(d1);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("OK");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void TestRead(MamanetFile file, string lorem, byte[] data)
        {
            Console.WriteLine("Trying to read file...");
            Console.Write("Reading Part 1... ");
            byte[] part1 = file[0].GetData();
            Console.WriteLine("Done!");
            Console.Write("Writing Part 2... ");
            byte[] part2 = file[1].GetData();
            Console.WriteLine("Done!");
            file.Close();
            Console.WriteLine("File is closed");
            Console.Write("Verifying... ");
            var newData = part1.Concat(part2).ToArray();
            if (newData.Length == data.Length && newData.Zip(data, (b1, b2) => b1 == b2).All(b => b))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed");
                Console.WriteLine(ASCIIEncoding.ASCII.GetString(newData.ToArray()));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(lorem);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void TestWrite(string filename, MamanetFile file, string lorem, byte[] data)
        {
            Console.WriteLine("Writing to {0}", filename);
            Console.Write("Writing Part 1... ");
            file[0].SetData(data.Take(1024).ToArray());
            Console.WriteLine("Done!");
            Console.Write("Writing Part 2... ");
            file[1].SetData(data.Skip(1024).ToArray());
            Console.WriteLine("Done!");
            file.Close();
            Console.WriteLine("File is closed");
            Console.Write("Verifying... ");
            if (File.ReadAllText(filename) == lorem)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
