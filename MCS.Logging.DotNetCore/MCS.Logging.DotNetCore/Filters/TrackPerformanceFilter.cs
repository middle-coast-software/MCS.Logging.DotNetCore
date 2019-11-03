using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Logging.DotNetCore.Filters
{
    public class TrackPerformanceFilter : IActionFilter
    {
        private PerfTracker _tracker;
        private readonly string _product, _layer;
        public TrackPerformanceFilter(string product, string layer)
        {
            _product = product;
            _layer = layer;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var activity = $"{request.Path}-{request.Method}";

            var dict = new Dictionary<string, object>();
            foreach (var key in context.RouteData.Values?.Keys)
                dict.Add($"RouteData-{key}", (string)context.RouteData.Values[key]);

            var details = McsWebHelper.GetWebFlogDetail(_product, _layer, activity,
                context.HttpContext, dict);

            _tracker = new PerfTracker(details);
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (_tracker != null)
                _tracker.Stop();
        }
    }
}
