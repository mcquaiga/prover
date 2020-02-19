using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Wpf.Startup;
using Hocon.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Client.Wpf
{
    public class AppBootstrapper
    {
        public static CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        public IConfiguration Config { get; set; }

        public IHost AppHost { get; set; }

        public void AddServices(IServiceCollection services)
        {
            Settings.AddServices(services, Config);
            Storage.AddServices(services, Config);
            UserInterface.AddServices(services, Config);
            DeviceServices.AddServices(services, Config);

            UpdaterService.AddServices(services, Config);
        }

        public void ConfigureAppConfiguration(IConfigurationBuilder config)
        {
            
            config.SetBasePath(Directory.GetCurrentDirectory());

            
        }

        public static async Task<AppBootstrapper> StartAsync(string[] args)
        {
            var booter = new AppBootstrapper();
            booter.AppHost = ConfigureBuilder(booter, args);

            await booter.AppHost.Services.GetServices<IHaveStartupTask>()
                .ToObservable()
                .ForEachAsync(t => t.StartAsync(CancellationTokenSource.Token));

            await booter.AppHost.StartAsync();

            return booter;
        }

        private static IHost ConfigureBuilder(AppBootstrapper booter, string[] args) =>
            Host.CreateDefaultBuilder()
                
                .ConfigureLogging(log => {

                    //log.ClearProviders();
                    //log.AddDebug();
                })
                
                .ConfigureAppConfiguration(builder =>
                {
                   
                    booter.ConfigureAppConfiguration(builder);
                    booter.Config = builder.Build();
                  
                })

                .ConfigureServices(booter.AddServices)
                .UseEnvironment(Environments.Development)
                .Build();
    }
}