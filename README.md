# MCS.Logging.DotNetCore
Simple global logging library for Serilog in .Net Core


## Setting Up
First You'll need a handlful of Environment Variables.

First grab the [nuget package](https://www.nuget.org/packages/MCS.Logging.DotNetCore)


### File Logging


### SQL Logging


### Helper Class
For better control over your logging you'll want to build a helper class.
There are a number of ways to write this, but here are a few examples....pending



## How To Actually Log Sh*...stuff
Now for the good stuff, it's time to hook all the pipes together.
In your startup class you'll need to register a global exception handler. 
Somthing along these lines should make it's way into your Startup class's Configure method.
```
app.UseExceptionHandler(axc =>
            {
                exc.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorCtx = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorCtx != null)
                    {
                        var ex = errorCtx.Error;
                        WebHelper.LogWebError("Product Name", "Api Name", ex, context);

                        var errorId = Activity.Current?.Id ?? context.TraceIdentifier;
                        var jsonResponse = JsonConvert.SerializeObject(new CustomErrorResponse
                        {
                            ErrorId = errorId,
                            Message = "Some kind of error happened in the API."
                        });
                        await context.Response.WriteAsync(jsonResponse, Encoding.UTF8);
                    }
                });
            });
```
