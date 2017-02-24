using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Common.LogUtilities
{
    public static class LogStoreProvider
    {
        private static IFormatter BinaryFormatter { get; set; }
        private static string FileName = "MamaNetLogStore.bin";

        public static List<LogEntry> LoadLogs()
        {
            var savedLogEntries = new List<LogEntry>();
            using (var fileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
            {
                BinaryFormatter = new BinaryFormatter();
                while (fileStream.Position != fileStream.Length)
                {
                    savedLogEntries.Add((LogEntry)BinaryFormatter.Deserialize(fileStream));
                }

            }
            return savedLogEntries;
        }

        public static void AppendLog(string entry, LogSeverity logSeverity)
        {
            var logEntry = new LogEntry() { Entry =  entry, LogSeverity = logSeverity, Timestamp = DateTime.Now};
            
            using (var fileStream = new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter = new BinaryFormatter();
                BinaryFormatter.Serialize(fileStream, logEntry);
            }
        }
    }
}
