using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public enum LogSeverity
    {
        Info,
        Error,
        CriticalError
    }

    public class Logger
    {
        private static string FilePath = Directory.GetCurrentDirectory()+"\\"+"MamaNet.log";

        public static void WriteLogEntry(string entry, LogSeverity severity)
        {
            var logEntry = string.Format("[{0}] ({1}): {2}{3}", severity, DateTime.Now, entry, Environment.NewLine);
            File.AppendAllText(FilePath, logEntry);
        }
    }
}
