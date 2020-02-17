using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Client.Wpf.Startup;
using Devices.Communications.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

namespace Client.Wpf
{
    public class AppBootstrapper
    {
        public IConfiguration Config { get; set; }

        public IHost AppHost { get; set; }

        public void AddServices(IServiceCollection services)
        {
            Settings.AddServices(services, Config);
            Storage.AddServices(services, Config);
            UserInterface.AddServices(services, Config);
            DeviceServices.AddServices(services, Config);
        }

        public void ConfigureAppConfiguration(IConfigurationBuilder configHost)
        {
            var fp = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            configHost.SetBasePath(Directory.GetCurrentDirectory());

            // configHost.AddJsonFile("appsettings.json", false);
            //configHost.AddEnvironmentVariables(prefix: "PREFIX_");
            //configHost.AddCommandLine(args);
        }

        public static async Task<AppBootstrapper> StartAsync(string[] args)
        {
            var booter = new AppBootstrapper();

            booter.AppHost = await Host
                .CreateDefaultBuilder()
                .ConfigureLogging(log =>
                {
                    log.ClearProviders();
                    log.AddDebug();
                })
                .ConfigureAppConfiguration(builder =>
                {
                    booter.ConfigureAppConfiguration(builder);
                    booter.Config = builder.Build();
                })
                .ConfigureServices(services => { booter.AddServices(services); })
                .UseEnvironment(Environments.Development)
                .StartAsync();


            return booter;
        }
    }
}