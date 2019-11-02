using MCS.Logging.DotNetCore.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Logging.DotNetCore.Builders
{
    public static class FileLogBuilder
    {
        internal static void BuildLogger(ref Serilog.ILogger _perfLogger, ref Serilog.ILogger _usageLogger, ref Serilog.ILogger _errorLogger, ref Serilog.ILogger _diagnosticLogger, McsLoggingSettings _settings)
        {
            _perfLogger = new LoggerConfiguration()
               .WriteTo.File(path: $"{_settings.LogFolderLocation}\\perf-{DateTime.Now.ToString("MMddyyyy")}.txt")
               .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{_settings.LogFolderLocation}\\usage-{DateTime.Now.ToString("MMddyyyy")}.txt")
                .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{_settings.LogFolderLocation}\\error-{DateTime.Now.ToString("MMddyyyy")}.txt")
                .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                .WriteTo.File(path: $"{_settings.LogFolderLocation}\\diagnostic-{DateTime.Now.ToString("MMddyyyy")}.txt")
                .CreateLogger();
        }
    }
}
