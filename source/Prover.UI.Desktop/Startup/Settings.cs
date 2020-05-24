using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application;
using Prover.Application.Services;
using Prover.Application.Settings;
using Prover.Shared.Interfaces;
using Prover.Shared.Storage.Interfaces;

namespace Prover.UI.Desktop.Startup {
	public class Settings : IHostedService {
		private readonly IServiceProvider _provider;

		public Settings(IServiceProvider provider) {
			_provider = provider;
		}

		public static void AddServices(IServiceCollection services, HostBuilderContext host) {
			services.AddSingleton(c => ApplicationSettings.Initialize(AppDefaults.Settings.FilePath, c.GetService<IKeyValueStore>()));

			services.AddHostedService<Settings>();
		}

		public Task StartAsync(CancellationToken cancellationToken) {
			return _provider.GetService<ISettingsService>().RefreshSettings();
			//return Task.CompletedTask;
		}

		public async Task StopAsync(CancellationToken cancellationToken) {
			await _provider.GetService<ISettingsService>().SaveSettings();
			//return Task.CompletedTask;
		}
	}
}