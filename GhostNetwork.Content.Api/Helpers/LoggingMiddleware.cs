using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GhostNetwork.Content.Api.Helpers;

public class LoggingMiddleware
{
    private readonly RequestDelegate next;

    public LoggingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext httpContext, ILogger<LoggingMiddleware> logger)
    {
        var sw = Stopwatch.StartNew();
        using var scopeStart = logger.BeginScope(new Dictionary<string, object>
        {
            ["type"] = "incoming:http"
        });
        logger.LogInformation($"{httpContext.Request.Method} {httpContext.Request.Path.Value}{httpContext.Request.QueryString.Value} request started");

        await next(httpContext);

        sw.Stop();
        using var scopeEnd = logger.BeginScope(new Dictionary<string, object>
        {
            ["type"] = "incoming:http",
            ["elapsedMilliseconds"] = sw.ElapsedMilliseconds
        });
        logger.LogInformation($"{httpContext.Request.Method} {httpContext.Request.Path.Value}{httpContext.Request.QueryString.Value} request finished");
    }
}