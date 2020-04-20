using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.Common;
using Prover.UI.Desktop.Extensions;
using Prover.UI.Desktop.Startup;

namespace Prover.UI.Desktop
{
    public class AppBootstrapper : IHostApplicationLifetime
    {
        public static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private ILogger _logger = NullLogger.Instance;

        public IHost AppHost { get; private set; }

        public CancellationToken ApplicationStarted { get; } = new CancellationToken();
        public CancellationToken ApplicationStopping { get; } = new CancellationToken();
        public CancellationToken ApplicationStopped { get; } = new CancellationToken();

        public async Task<AppBootstrapper> StartAsync(string[] args)
        {
            AppHost = ConfigureBuilder(this, args);

            await AppHost.StartAsync(ApplicationStarted);
            //await ExecuteStartUpTasks();

            InitializeLogging();

            return this;
        }

        public void StopApplication()
        {
            Debug.WriteLine("Stopping Application.");
            AppHost.StopAsync(ApplicationStopped);
        }

        private void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            services.AddSingleton<ISchedulerProvider, SchedulerProvider>();
            services.AddSingleton<UnhandledExceptionHandler>();

            Settings.AddServices(services, host);
            StorageStartup.AddServices(services, host);
            UserInterface.AddServices(services, host);
            DeviceServices.AddServices(services, host);
            UpdaterService.AddServices(services, host);
        }

        private static IHost ConfigureBuilder(AppBootstrapper booter, string[] args) =>
                Host.CreateDefaultBuilder()
                    .ConfigureLogging((host, log) =>
                    {
                        log.Services.AddSplatLogging();

                        if (host.HostingEnvironment.IsProduction()) log.AddEventLog();

                        if (host.HostingEnvironment.IsProduction() == false) log.AddDebug();
                    })
                    .ConfigureServices((host, services) =>
                    {
                        booter.AddServices(services, host);
                        host.ConfigureModules(services);
                    })
                    .Build();

        private async Task ExecuteStartUpTasks()
        {
            //var startTasks = AppHost.Services.GetServices<IStartupTask>()
            //                        .Select(t =>
            //                                t.ExecuteAsync(CancellationTokenSource.Token))
            //                        .ForEach(t => t.Start())
            //                        .ToArray();

            ////startTasks.ForEach(t => t.Start());
            //await Task.WhenAll(startTasks);

            ////if (startTasks.Any(t => t.IsFaulted))
            //    foreach (var ex in startTasks.Where(t => t.IsFaulted))
            //    {
            //        ex.Exception.InnerExceptions.ForEach(x =>
            //                _logger.LogError(x, "An error occured on start up."));
            //    }

                await Task.CompletedTask;
        }

        private void InitializeLogging()
        {
            var logFactory = AppHost.Services.GetService<ILoggerFactory>();
            ProverLogging.LogFactory = logFactory;

            _logger = logFactory.CreateLogger<AppBootstrapper>();

            var config = AppHost.Services.GetService<IConfiguration>();
            var host = AppHost.Services.GetService<IHostEnvironment>();

            _logger.LogInformation($"Loggers Hosting App Name: {host.ApplicationName}");
            _logger.LogInformation($"Loggers Hosting Env: {host.EnvironmentName}");

            var level = config.GetValue<LogLevel>("Logging:LogLevel:Default");
            _logger.LogInformation($"Log Level = {level}");
        }
    }
}