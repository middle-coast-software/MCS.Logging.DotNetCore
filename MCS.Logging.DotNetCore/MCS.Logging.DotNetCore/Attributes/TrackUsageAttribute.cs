﻿using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Logging.DotNetCore.Attributes
{
    public class TrackUsageAttribute : ActionFilterAttribute
    {
        private string _product, _layer, _activityName;

        public TrackUsageAttribute(string product, string layer, string activityName)
        {
            _product = product;
            _layer = layer;
            _activityName = activityName;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var dict = new Dictionary<string, object>();
            foreach (var key in context.RouteData.Values?.Keys)
                dict.Add($"RouteData-{key}", (string)context.RouteData.Values[key]);

            McsWebHelper.LogWebUsage(_product, _layer, _activityName, context.HttpContext, dict);
        }
    }
}
