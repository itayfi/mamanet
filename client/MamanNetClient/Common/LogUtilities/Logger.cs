using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.LogUtilities
{

    public class Logger
    {
        private static readonly string FilePath = Directory.GetCurrentDirectory()+"\\"+"MamaNet.log";
        private static readonly object LogLock = new Object();

        public static void WriteLogEntry(string entry, LogSeverity severity)
        {
            var logEntry = string.Format("[{0}] ({1}): {2}{3}", severity, DateTime.Now, entry, Environment.NewLine);

            lock (LogLock)
            {
                File.AppendAllText(FilePath, logEntry);
                LogStoreProvider.AppendLog(entry,severity);
            }
        }
    }
}
