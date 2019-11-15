# MCS.Logging.DotNetCore
Simple global logging library for Serilog in .Net Core

Logging should be simple, that's where this library comes in. It's built on serilog to enable SLQ or File Logging (or both) with as little setup as possible. No need to pick NuGet packages by ones and twos and no need to write all the supporting classes for global exceptions or performance tracking on ASP.Net classes.  This library will work for both APIs and ASP.NET web applications (they're essentially the same in .NET Core anyway).

Note to user: This was first developed for .Net Core 3.0 so there was not 2.2 support planned, however I did have that need after developing this so did throw one together. YOu can find it in NuGet under [Mcs.Logging.Legacy2_2](https://www.nuget.org/packages/MCS.Logging.Legacy2_2/)



## Setting Up
First grab the [nuget package](https://www.nuget.org/packages/MCS.Logging.DotNetCore)

### Choosing between File and SQL Logging
This is achieved with an environment variable called MCS_LOG_DESTINATION_TYPES.
Writing to file is achieved by setting this to: Log
Writing to SQL is turned on by setting this to: SQL
Writing to both is turned on by listing them: Log SQL (or SQL Log order doesn't matter)

### General
Diagnostic logging is something that's not always desired in all environments. It is enabled with an environment variable called MCS_ENABLE_DIAGNOSTICS.
It requires values of either True or False. (I believe it is case insensitive, but if I'm totally honest I haven't tested it sufficiently.)

### File Logging
You'll need another environment variable.

MCS_LOG_FOLDER_LOCATION

Give it the location where you would like your log files to be generated. I like setting it to: .\Logs
That keeps it local to my applications when in dev. It supports both relative and absolute paths.

The library will automatically generate files, by date, for diagnostic-<-date-) (empty or nonexistent if MCS_ENABLE_DIAGNOSTICS is false), error-<-date-), perf-<-date-), and usage-<-date-).


### SQL Logging
With SQL logging you need two Environment Variables
MCS_LOG_CONNECTION ==> a connection string
MCS_LOG_BATCH_SIZE ==> an integer

Log batch size defaults to 1, but if you have high throughput environments you may wish to commit these to a db in batches.

And if your database doesn't already have tables to accept the data, they will be created for you. This assumes the connection string provided is with a user that has sufficient rights for a code first migration, which has been appropriate in my use cases.
If you need sql scripts to create them yourself, post up an issue and I'll add them. (I haven't needed them so that's work I don't know is necessary just yet.)


## How To Actually Log Sh*...stuff
Now for the good stuff, it's time to hook all the pipes together.
In your startup class you'll need to register a global exception handler. 

For an API this is pretty simple. Unless you for some reason need to log performance or usage, but we'll get to that.
#### Error Logging

Somthing along these lines should make it's way into your Startup class's Configure method. It will write to your error file or table

```
app.UseExceptionHandler(exc =>
    {
        exc.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            Console.WriteLine("getting context");

            var errorCtx = context.Features.Get<IExceptionHandlerFeature>();

            if (errorCtx != null)
            {
                var ex = errorCtx.Error;
                Console.WriteLine("writing log");
                McsWebHelper.LogWebError("Product Name", "Api Name", ex, context);

                var errorId = Activity.Current?.Id ?? context.TraceIdentifier;
                var jsonResponse = JsonConvert.SerializeObject(new McsErrorResponse
                {
                    ErrorId = errorId,
                    Message = "Some kind of error happened in the API."
                });
                await context.Response.WriteAsync(jsonResponse, Encoding.UTF8);
            }
        });
    });
```

#### Performance Tracking
Add a line to your services.AddMvc() code.
'''

services.AddMvc(options =>
    {
        options.EnableEndpointRouting = false;
        options.Filters.Add(new TrackPerformanceFilter("SampleAPI", "API"));
    }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

'''

#### Usage Tracking
You should be able to add a method attribute to your controller classes.
Something like this should do it.
'''
[TrackUsage("Controller Name", "App MVC", "action description")]
'''


#### Diagnostic Logging
Diagnostic logging is supported with static helper that you can add wherever needed. Note that it does need an HttpContext, so it's not meant to be put just anywhere, but we are talking about web apps here so it didn't feel like a stretch.
'''
    McsWebHelper.LogWebDiagnostic("product name", "application layer", "some message", currentHttpContext, <-Dictionary<string, object> to be submitted for storage.->)
'''