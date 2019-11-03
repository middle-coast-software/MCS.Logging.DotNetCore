using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Logging.DotNetCore.Settings
{
    public class McsLoggingSettings
    {
        public bool EnableDiagnostics { get; set; } = false;
        public string McsLogDestinationTypes { get; set; }
        public string LogFolderLocation { get; set; }
        public string LogConnection { get; set; }
        public int LogBatchSize { get; set; } = 1;
    }
}
