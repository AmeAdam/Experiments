using System.Collections;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GiadaServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var isDebug = Debugger.IsAttached || ((IList) args).Contains("--debug");
            var runPath = isDebug ? Directory.GetCurrentDirectory() : Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            var config = new ConfigurationBuilder()
                .SetBasePath(runPath)
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(runPath)
                .UseConfiguration(config) //.UseUrls("http://localhost:8080", "http://*:8080", "http://0.0.0.0:8080")
                .UseStartup<Startup>()
                .Build();

            using (var mainService = host.Services.GetService<IGiadaMainService>())
            {
                mainService.Run(host, isDebug, args);
            }
        }
    }
}
