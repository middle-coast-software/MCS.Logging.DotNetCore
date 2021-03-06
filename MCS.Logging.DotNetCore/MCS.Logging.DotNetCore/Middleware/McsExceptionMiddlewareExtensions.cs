﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MCS.Logging.DotNetCore.Middleware
{
    public static class McsExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(
            this IApplicationBuilder builder, string product, string layer,
            string errorHandlingPath)
        {
            return builder.UseMiddleware<McsExceptionHandlerMiddleware>
                (product, layer, Options.Create(new ExceptionHandlerOptions
                {
                    ExceptionHandlingPath = new PathString(errorHandlingPath)
                }));
        }
    }
}
