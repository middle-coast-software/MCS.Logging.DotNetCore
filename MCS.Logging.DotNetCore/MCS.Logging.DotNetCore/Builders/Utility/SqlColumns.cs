using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace MCS.Logging.DotNetCore.Builders.Utility
{
    public static class SqlColumns
    {
        public static ColumnOptions GetSqlColumnOptions()
        {
            var colOptions = new ColumnOptions();
            colOptions.Store.Remove(StandardColumn.Properties);
            colOptions.Store.Remove(StandardColumn.MessageTemplate);
            colOptions.Store.Remove(StandardColumn.Message);
            colOptions.Store.Remove(StandardColumn.Exception);
            colOptions.Store.Remove(StandardColumn.TimeStamp);
            colOptions.Store.Remove(StandardColumn.Level);

            colOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn {DataType = SqlDbType.DateTime, ColumnName = "Timestamp"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Product"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Layer"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Location"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Message"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Hostname"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "UserId"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "UserName"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Exception"},
                new SqlColumn {DataType = SqlDbType.Int, ColumnName = "ElapsedMilliseconds"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "CorrelationId"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "CustomException"},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "AdditionalInfo"},
            };

            return colOptions;
        }
    }
}
