using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Application;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.Common;
using Prover.UI.Desktop.Extensions;
using Prover.UI.Desktop.Startup;
using Prover.Updater;

namespace Prover.UI.Desktop {
	public class AppBootstrapper : IDisposable
	//		: IHostApplicationLifetime 
	{
		public static CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
		private readonly ILogger<AppBootstrapper> _logger;

		public AppBootstrapper(IServiceProvider provider, IHost appHost) {
			_logger = provider.GetService<ILogger<AppBootstrapper>>();
			Provider = provider;
			AppHost = appHost;

			LifetimeHost = Provider.GetService<IHostApplicationLifetime>();
			//LifetimeHost.ApplicationStarted.Register(() => Task.Run(() => OnApplicationStarted()), true);
			//LifetimeHost.ApplicationStopping.Register(OnApplicationStopping, true);
			//LifetimeHost.ApplicationStopped.Register(OnApplicationStopped, true);
		}

		public static AppBootstrapper Instance { get; private set; }

		public IServiceProvider Provider { get; }
		public IHost AppHost { get; }
		public IHostApplicationLifetime LifetimeHost { get; }

		public static AppBootstrapper Configure(string[] args) {
			var host = ConfigureBuilder(args);
			Instance = ActivatorUtilities.CreateInstance<AppBootstrapper>(host.Services);
			return Instance;
		}

		public static void ConfigureAppServices(HostBuilderContext host, IServiceCollection services) {
			services.AddSingleton<ISchedulerProvider, SchedulerProvider>();
			services.AddSingleton<UnhandledExceptionHandler>();

			Settings.AddServices(services, host);
			StorageStartup.AddServices(services, host);
			UserInterface.AddServices(services, host);
			DeviceServices.AddServices(services, host);

			services.UpdateService(host);
			//#endif
		}

		public static async Task StartAsync(string[] args) {
			Configure(args);
			await Instance.StartAsync();
			await Instance.OnApplicationStarted();
		}

		public Task StartAsync() => AppHost.StartAsync(CancellationTokenSource.Token);

		public void StopAsync() {
			_logger.LogDebug("Stopping Application ...");

			if (CancellationTokenSource.IsCancellationRequested)
				CancellationTokenSource = new CancellationTokenSource();

			var token = CancellationTokenSource.Token;

			var services = AppHost.Services.GetServices<IHostedService>();
			var t = Task.WhenAll(services.Select(s => Task.Run(() => s.StopAsync(new CancellationToken()))));
			t.Wait();
			//return Task.CompletedTask;
		}

		private static IHost ConfigureBuilder(string[] args) {
			var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
			if (string.IsNullOrWhiteSpace(environment))
				environment = "Production";

			var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
												   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
												   .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
												   .AddEnvironmentVariables("DOTNET_")
												   .AddCommandLine(args)
												   .Build();

			var builder = Host.CreateDefaultBuilder()
							  .ConfigureHostConfiguration(c => c.AddConfiguration(config))
							  .ConfigureAppConfiguration((host, config) => {
								  host.DiscoverModules();
								  host.AddModuleConfigurations(config);
							  })
							  .ConfigureLogging((host, log) => {
								  log.ClearProviders();
								  log.Services.AddSplatLogging();

								  if (host.HostingEnvironment.IsProduction())
									  log.AddEventLog();
								  else
									  log.AddDebug();
							  })
							  .ConfigureServices((host, services) => {
								  host.AddModuleServices(services);
								  ConfigureAppServices(host, services);
							  })
		;
			var host = builder.Build();
			host.Services.UseMicrosoftDependencyResolver();
			return host;


			//.ConfigureHostConfiguration(config => {
			// host.DiscoverModules();
			//})
			//.ConfigureHostConfiguration(config => {
			// host.AddModuleConfigurations(config);

			// config.AddJsonFile("appsettings.json");
			// config.AddJsonFile("appsettings.Development.json");
			//})
			//.ConfigureAppConfiguration((host, config) => {
			// host.DiscoverModules(config);
			// host.AddModuleConfigurations(config);
			//})
		}

		private Task OnApplicationStarted() {
			ProverLogging.Initialize(AppHost.Services);
			return Task.CompletedTask;
		}


		/// <inheritdoc />
		public void Dispose() {
			AppHost?.Dispose();
		}
	}
}