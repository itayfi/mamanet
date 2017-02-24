using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.LogUtilities
{
    public enum LogSeverity
    {
        Info,
        Error,
        CriticalError
    }

    [Serializable]
    public class LogEntry
    {
        public string Entry { get; set; }
        public LogSeverity LogSeverity { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
