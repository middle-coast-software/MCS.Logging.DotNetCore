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
                new SqlColumn {DataType = SqlDbType.DateTime, ColumnName = "Timestamp", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Product", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Layer", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Location", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Message", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Hostname", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "UserId", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "UserName", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "Exception", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.Int, ColumnName = "ElapsedMilliseconds", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "CorrelationId", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "CustomException", AllowNull = true},
                new SqlColumn {DataType = SqlDbType.VarChar, ColumnName = "AdditionalInfo", AllowNull = true},
            };

            return colOptions;
        }
    }
}
