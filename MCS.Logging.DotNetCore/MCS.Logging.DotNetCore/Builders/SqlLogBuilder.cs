using MCS.Logging.DotNetCore.Builders.Utility;
using MCS.Logging.DotNetCore.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Logging.DotNetCore.Builders
{
    public static class SqlLogBuilder
    {
        internal static void BuildLogger(ref Serilog.ILogger _perfLogger, ref Serilog.ILogger _usageLogger, ref Serilog.ILogger _errorLogger, ref Serilog.ILogger _diagnosticLogger, McsLoggingSettings _settings)
        {
            _perfLogger = new LoggerConfiguration()
               .WriteTo.MSSqlServer(_settings.LogConnection, "PerfLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: _settings.LogBatchSize)
               .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(_settings.LogConnection, "UsageLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: _settings.LogBatchSize)
                .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(_settings.LogConnection, "ErrorLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: _settings.LogBatchSize)
                .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(_settings.LogConnection, "DiagnosticLogs", autoCreateSqlTable: true,
                    columnOptions: SqlColumns.GetSqlColumnOptions(), batchPostingLimit: _settings.LogBatchSize)
                .CreateLogger();
        }
    }
}
