using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Logging.DotNetCore.Attributes
{
    public sealed class TrackUsageAttribute : ActionFilterAttribute
    {
        private readonly string _product, _layer, _activityName;
        private readonly McsLogger _logger;

        public TrackUsageAttribute(McsLogger logger, string product, string layer, string activityName)
        {
            _product = product;
            _layer = layer;
            _activityName = activityName;
            _logger = logger;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var dict = new Dictionary<string, object>();
            foreach (var key in context.RouteData.Values?.Keys)
                dict.Add($"RouteData-{key}", (string)context.RouteData.Values[key]);

            McsWebHelper.LogWebUsage(_logger, _product, _layer, _activityName, context.HttpContext, dict);
        }
    }
}
