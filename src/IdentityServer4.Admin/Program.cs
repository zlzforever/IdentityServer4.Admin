using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace IdentityServer4.Admin
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configFile = args.FirstOrDefault(a => a.Contains("appsettings.json"));
            if (!File.Exists(configFile))
            {
                Console.WriteLine($@"File: {configFile} not exists");
            }

            var mountFolder = configFile != null && File.Exists(configFile)
                ? Path.GetDirectoryName(configFile)
                : "";

            var logDirectory = new DirectoryInfo(Path.Combine(mountFolder, "log"));
            if (!logDirectory.Exists)
            {
                logDirectory.Create();
            }

            var logFile = Path.Combine(mountFolder, "log", "ids4-admin.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console().WriteTo.RollingFile(logFile,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            Log.Logger.Information($"Arguments: {string.Join(" ", args)}");
            Log.Logger.Information($"Log to: {logFile}");
            var builder = WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration(config =>
                {
                    var configPath = Path.Combine(mountFolder, "appsettings.json");
                    config.AddJsonFile(configPath);
                    Log.Logger.Information($"Use config: {configPath}");
                })
                .UseStartup<Startup>().UseSerilog().UseUrls("http://0.0.0.0:6566");

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

            builder.UseSetting("MountFolder", mountFolder);

            var host = builder.Build();
            host.Run();
        }
    }
}