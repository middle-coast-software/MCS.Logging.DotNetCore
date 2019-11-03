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
    public static class McsLogger
    {
        private static readonly ILogger _perfLogger;
        private static readonly ILogger _usageLogger;
        private static readonly ILogger _errorLogger;
        private static readonly ILogger _diagnosticLogger;
        private static readonly McsLoggingSettings _settings;

        static McsLogger()
        {
            if (!bool.TryParse(Environment.GetEnvironmentVariable("MCS_ENABLE_DIAGNOSTICS"), out var enableDiagnostics))
                enableDiagnostics = false;
            if (!int.TryParse(Environment.GetEnvironmentVariable("MCS_LOG_BATCH_SIZE"), out var batchSize))
                batchSize = 1;

            _settings = new McsLoggingSettings()
            {
                EnableDiagnostics = enableDiagnostics,
                LogBatchSize = batchSize,
                LogConnection = Environment.GetEnvironmentVariable("MCS_LOG_CONNECTION"),
                LogFolderLocation = Environment.GetEnvironmentVariable("MCS_LOG_FOLDER_LOCATION"),
                McsLogDestinationTypes = Environment.GetEnvironmentVariable("MCS_LOG_DESTINATION_TYPES")
            };
            
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

        public static void WritePerf(LogDetail infoToLog)
        {
            _perfLogger.Write(LogEventLevel.Information,
                       "{Timestamp}{Product}{Layer}{Location}{Message}{Hostname}{UserId}{UserName}{Exception}{ElapsedMilliseconds}{CorrelationId}{CustomException}{AdditionalInfo}",
                        infoToLog.Timestamp,
                        infoToLog.Product, infoToLog.Layer, infoToLog.Location, infoToLog.Message, infoToLog.Hostname,
                        infoToLog.UserId, infoToLog.UserName, infoToLog.Exception?.ToBetterString(),
                        infoToLog.ElapsedMilliseconds,
                        infoToLog.CorrelationId,
                        infoToLog.Exception,
                        infoToLog.AdditionalInfo
                        );
        }
        public static void WriteUsage(LogDetail infoToLog)
        {
            _usageLogger.Write(LogEventLevel.Information,
                    "{Timestamp}{Product}{Layer}{Location}{Message}{Hostname}{UserId}{UserName}{Exception}{ElapsedMilliseconds}{CorrelationId}{CustomException}{AdditionalInfo}",
                        infoToLog.Timestamp,
                        infoToLog.Product, infoToLog.Layer, infoToLog.Location, infoToLog.Message, infoToLog.Hostname,
                        infoToLog.UserId, infoToLog.UserName, infoToLog.Exception?.ToBetterString(),
                        infoToLog.ElapsedMilliseconds,
                        infoToLog.CorrelationId,
                        infoToLog.CustomException,
                        infoToLog.AdditionalInfo
                        );
        }
        public static void WriteError(LogDetail infoToLog)
        {
            if (infoToLog.Exception != null)
            {
                var procName = FindProcName(infoToLog.Exception);
                infoToLog.Location = string.IsNullOrEmpty(procName)
                    ? infoToLog.Location
                    : procName;
                infoToLog.Message = GetMessageFromException(infoToLog.Exception);
            }
            _errorLogger.Write(LogEventLevel.Error,
                    "{Timestamp}{Product}{Layer}{Location}{Message}{Hostname}{UserId}{UserName}{Exception}{ElapsedMilliseconds}{CorrelationId}{CustomException}{AdditionalInfo}",
                    infoToLog.Timestamp,
                    infoToLog.Product, infoToLog.Layer, infoToLog.Location, infoToLog.Message, infoToLog.Hostname,
                    infoToLog.UserId, infoToLog.UserName, infoToLog.Exception?.ToBetterString(),
                    infoToLog.ElapsedMilliseconds,
                    infoToLog.CorrelationId,
                    infoToLog.CustomException,
                    infoToLog.AdditionalInfo
                    );
        }
        public static void WriteDiagnostic(LogDetail infoToLog)
        {
            var writeDiagnostics =
                Convert.ToBoolean(Environment.GetEnvironmentVariable("DIAGNOSTICS_ON"));
            if (!writeDiagnostics)
                return;

            _diagnosticLogger.Write(LogEventLevel.Information,
                  "{Timestamp}{Product}{Layer}{Location}{Message}{Hostname}{UserId}{UserName}{Exception}{ElapsedMilliseconds}{CorrelationId}{CustomException}{AdditionalInfo}",
                       infoToLog.Timestamp,
                       infoToLog.Product, infoToLog.Layer, infoToLog.Location, infoToLog.Message, infoToLog.Hostname,
                       infoToLog.UserId, infoToLog.UserName, infoToLog.Exception?.ToBetterString(),
                       infoToLog.ElapsedMilliseconds,
                       infoToLog.CorrelationId,
                       infoToLog.CustomException,
                       infoToLog.AdditionalInfo
                       );
        }

        private static string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetMessageFromException(ex.InnerException);

            return ex.Message;
        }

        private static string FindProcName(Exception ex)
        {
            if (ex is SqlException sqlEx)
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