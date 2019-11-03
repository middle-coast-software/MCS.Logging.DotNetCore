﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Logging.DotNetCore.Models
{
    public class LogDetail
    {
        public LogDetail()
        {
            Timestamp = DateTime.Now;
        }
        public DateTime Timestamp { get; private set; }
        public string Message { get; set; }
        // WHERE
        public string Product { get; set; }
        public string Layer { get; set; }
        public string Location { get; set; }
        public string Hostname { get; set; }

        // WHO
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        // EVERYTHING ELSE
        public long? ElapsedMilliseconds { get; set; }  // only for performance entries
        public Exception Exception { get; set; }  // the exception for error logging
        public string CorrelationId { get; set; } // exception shielding from server to client
        public Dictionary<string, object> AdditionalInfo { get; set; }  // everything else
        public McsException CustomException { get; set; }
    }
}
