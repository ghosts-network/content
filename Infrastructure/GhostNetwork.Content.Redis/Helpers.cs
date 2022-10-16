using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GhostNetwork.Content.Redis;

public static class Helpers
{
    public static async Task<TResult> LogExecutionInfo<TResult, TContext>(this ILogger<TContext> logger, Func<Task<TResult>> func)
    {
        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["type"] = "outgoing:redis"
               }))
        {
            logger.LogInformation("Redis query started");

            var sw = Stopwatch.StartNew();
            var result = await func();

            sw.Stop();
            using (logger.BeginScope(new Dictionary<string, object>
                   {
                       ["elapsedMilliseconds"] = sw.ElapsedMilliseconds
                   }))
            {
                logger.LogInformation("Redis query finished");
            }

            return result;
        }
    }
}