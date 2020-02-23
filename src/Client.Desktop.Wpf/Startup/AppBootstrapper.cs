using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Wpf.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Client.Wpf.Startup
{
    public class AppBootstrapper
    {
        public static CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        public IConfiguration Config { get; set; }

        public IHost AppHost { get; set; }

        public void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            Settings.AddServices(services, host);
            Storage.AddServices(services, host);
            UserInterface.AddServices(services, host);
            DeviceServices.AddServices(services, host);

            if (host.HostingEnvironment.EnvironmentName != Environments.Development)
            {
                //UpdaterService.AddServices(services, host);
            }
        }

        public void AddConfiguration(IConfigurationBuilder config, HostBuilderContext host)
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
        }


        public async Task<AppBootstrapper> StartAsync(string[] args)
        {
            AppHost = ConfigureBuilder(this, args);

            await AppHost.Services.GetServices<IHaveStartupTask>()
                .ToObservable()
                .ForEachAsync(t => t.ExecuteAsync(CancellationTokenSource.Token));

            await AppHost.StartAsync();

            return this;
        }

        private static IHost ConfigureBuilder(AppBootstrapper booter, string[] args) =>
            Host.CreateDefaultBuilder()
               
                .ConfigureLogging((host, log) => {

                    //log.ClearProviders();
                    log.AddDebug();
                })
                
                .ConfigureAppConfiguration((host, config) =>
                {
                    booter.AddConfiguration(config, host);
                })
                
                .ConfigureServices((host, services) =>
                {
                    host.ConfigureModules(services);

                    booter.AddServices(services, host);
                })
               
                .UseEnvironment(Environment.GetEnvironmentVariable("PROVER_ENVIRONMENT") ?? Environments.Production)
                
                .Build();
    }
}