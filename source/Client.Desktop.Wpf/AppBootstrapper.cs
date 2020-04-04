using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Common;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application;
using Prover.Shared.Interfaces;

namespace Client.Desktop.Wpf
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
            await ExecuteStartUpTasks();

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
            Storage.AddServices(services, host);
            UserInterface.AddServices(services, host);
            DeviceServices.AddServices(services, host);
            UpdaterService.AddServices(services, host);
        }

        private static IHost ConfigureBuilder(AppBootstrapper booter, string[] args) =>
            Host.CreateDefaultBuilder()
                .ConfigureLogging((host, log) =>
                {
                    //log.AddEventLog();
                    log.AddDebug();
                    log.Services.AddSplatLogging();
#if DEBUG

#endif
                })
                .ConfigureServices((host, services) =>
                {
                    booter.AddServices(services, host);
                    host.ConfigureModules(services);
                })
                .Build();

        private async Task ExecuteStartUpTasks()
        {
            var startTasks = AppHost.Services.GetServices<IStartupTask>().ToObservable()
                .ForEachAsync(async t => await t.ExecuteAsync(CancellationTokenSource.Token));

            await startTasks;

            if (startTasks.IsFaulted)
                foreach (var ex in startTasks.Exception.InnerExceptions)
                    _logger.LogError(ex, "An error occured on start up.");
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