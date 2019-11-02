using MCS.Logging.DotNetCore.Builders;
using MCS.Logging.DotNetCore.Models;
using MCS.Logging.DotNetCore.Settings;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MCS.Logging.DotNetCore
{
    public class McsLogger
    {
        private readonly ILogger _perfLogger;
        private readonly ILogger _usageLogger;
        private readonly ILogger _errorLogger;
        private readonly ILogger _diagnosticLogger;
        private readonly McsLoggingSettings _settings;

        public McsLogger(McsLoggingSettings settings)
        {
            _settings = settings;
            
            var logToFile = _settings.McsLogDestinationTypes.ToUpper().Contains("FILE");
            var logToSQL = _settings.McsLogDestinationTypes.ToUpper().Contains("SQL");

            if (logToSQL && logToFile)
            {
                FileSqlLogBuilder.BuildLogger(ref _perfLogger, ref _usageLogger, ref _errorLogger, ref _diagnosticLogger, _settings);
            }
            else if (logToFile)
                FileLogBuilder.BuildLogger(ref _perfLogger, ref _usageLogger, ref _errorLogger, ref _diagnosticLogger, _settings);
            else if (logToSQL)
            {
                SqlLogBuilder.BuildLogger(ref _perfLogger, ref _usageLogger, ref _errorLogger, ref _diagnosticLogger, _settings);
            }
        }

        public void WritePerf(LogDetail infoToLog)
        {
            _perfLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }
        public void WriteUsage(LogDetail infoToLog)
        {
            _usageLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }
        public void WriteError(LogDetail infoToLog)
        {
            if (infoToLog.Exception != null)
            {
                var procName = FindProcName(infoToLog.Exception);
                infoToLog.Location = string.IsNullOrEmpty(procName)
                    ? infoToLog.Location
                    : procName;
                infoToLog.Message = GetMessageFromException(infoToLog.Exception);
            }
            _errorLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }
        public void WriteDiagnostic(LogDetail infoToLog)
        {
            var writeDiagnostics =
                Convert.ToBoolean(Environment.GetEnvironmentVariable("DIAGNOSTICS_ON"));
            if (!writeDiagnostics)
                return;

            _diagnosticLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }

        private string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetMessageFromException(ex.InnerException);

            return ex.Message;
        }

        private string FindProcName(Exception ex)
        {
            var sqlEx = ex as SqlException;
            if (sqlEx != null)
            {
                var procName = sqlEx.Procedure;
                if (!string.IsNullOrEmpty(procName))
                    return procName;
            }

            if (!string.IsNullOrEmpty((string)ex.Data["Procedure"]))
            {
                return (string)ex.Data["Procedure"];
            }

            if (ex.InnerException != null)
                return FindProcName(ex.InnerException);

            return null;
        }
    }
}