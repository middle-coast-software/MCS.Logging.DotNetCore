# MCS.Logging.DotNetCore
Simple global logging library for Serilog in .Net Core


## Setting Up

First grab the [nuget package](https://www.nuget.org/packages/MCS.Logging.DotNetCore)

Then you'll need a handlful of Environment Variables.

### General
McsLogDestinationTypes

### File Logging
LogFolderLocation

### SQL Logging
LogConnection
LogBatchSize

### Helper Class
For better control over your logging you'll want to build a helper class.
There are a number of ways to write this, but here are a few examples.

General Logging - [WebHelper](.\Mcs.Logging.DotNetCore\Mcs.Logging.DotNetCore\McsWebHelper.cs)

More examples forthcoming.


## How To Actually Log Sh*...stuff
Now for the good stuff, it's time to hook all the pipes together.
In your startup class you'll need to register a global exception handler. 

For an API this is pretty simple. Unless you for some reason need to log performance or usage, but we'll get to that.

Somthing along these lines should make it's way into your Startup class's Configure method.

```
app.UseExceptionHandler(exc =>
                {
                    exc.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";

                        var errorCtx = context.Features.Get<IExceptionHandlerFeature>();
                        if (errorCtx != null)
                        {
                            var ex = errorCtx.Error;
                            McsWebHelper.LogWebError("Product Name", "Api Name", ex, context);

                            var errorId = Activity.Current?.Id ?? context.TraceIdentifier;
                            var jsonResponse = JsonConvert.SerializeObject(new McsErrorResponse
                            {
                                ErrorId = errorId,
                                //Your deployment environment and intended use will impact what you put here
                                Message = "Some kind of error happened in the API." 
                            });
                            await context.Response.WriteAsync(jsonResponse, Encoding.UTF8);
                        }
                    });
                });
```