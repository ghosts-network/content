using System;
using GhostNetwork.Content.Api.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Filters;

namespace GhostNetwork.Content.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.With<UtcTimestampEnricher>()
                .Enrich.FromLogContext()
                .Filter.ByIncludingOnly(Matching.FromSource("GhostNetwork"))
                .WriteTo.Console(outputTemplate: "{UtcTimestamp:yyyy-MM-ddTHH:mm:ss.ffffZ} [{Level:u3}] {Message:l} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            var startupLogger = Log.ForContext<Program>();

            try
            {
                var host = Host.CreateDefaultBuilder(args)
                    .UseSerilog()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    })
                    .Build();

                host.Start();
                startupLogger.Information("Starting http server on port 5010");

                host.WaitForShutdown();
                return 0;
            }
            catch (Exception ex)
            {
                startupLogger.Error(ex.Message);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}