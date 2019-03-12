using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using EnvironmentName = Microsoft.AspNetCore.Hosting.EnvironmentName;

namespace IdentityServer4.Admin
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string logFile = "ids4.log";
            if (Directory.Exists("/ids4admin"))
            {
                var logDirectory = new DirectoryInfo("/ids4admin/log");
                if (!logDirectory.Exists)
                {
                    logDirectory.Create();
                }

                logFile = "/ids4admin/log/ids4.log";
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console().WriteTo.RollingFile(logFile,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Logger.Information($"Log to: {logFile}");
            var builder = WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration(config =>
                {
                    var configFile = args.FirstOrDefault(a => a.Contains("appsettings.json"));
                    if (configFile != null && File.Exists(configFile))
                    {
                        config.AddJsonFile(configFile);
                        Log.Logger.Information($"Use external config: {configFile}");
                    }
                })
                .UseStartup<Startup>().UseSerilog().UseUrls("http://0.0.0.0:5566");

            var seed = args.Contains("/seed");
            if (seed)
            {
                builder.UseSetting("seed", "true");
            }

            if (args.Contains("/dev"))
            {
                builder.UseEnvironment(EnvironmentName.Development);
            }

            if (args.Contains("/prod"))
            {
                builder.UseEnvironment(EnvironmentName.Production);
            }

            var host = builder.Build();
            host.Run();
        }
    }
}