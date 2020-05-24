using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Octokit.Reactive;
using Prover.UI.Desktop.Common;

namespace Prover.Updater {
	public partial class UpdaterService : CronJobService {
		private const string AppSettingsReleasesConfigKey = "Releases:Path";
		private const string AppSettingsUpdateScheduleKey = "Releases:UpdateSchedule";
		private readonly ILogger<UpdaterService> _logger;
		private CancellationTokenSource _cancellationSource = new CancellationTokenSource();
		private readonly GitHubUpdateManager _gitHubUpdateManager;

		public UpdaterService(ILogger<UpdaterService> logger, IHostApplicationLifetime host, CronExpression schedule) : base(schedule) {
			_logger = logger ?? NullLogger<UpdaterService>.Instance;
			_gitHubUpdateManager = new GitHubUpdateManager();

			host.ApplicationStarted.Register(() => {
				//Task.Run(() => DoWork(_cancellationSource.Token));
			}, true);
		}

		public override Task DoWork(CancellationToken cancellationToken) {
			if (_gitHubUpdateManager.Status == HealthStatus.Critical) {
				return StopAsync(cancellationToken);
			}

			return CheckForUpdate(cancellationToken);
		}

		private Task CheckForUpdate(CancellationToken cancellationToken) {
			try {
				_logger.LogInformation("Checking for updates....");
				return _gitHubUpdateManager.Update(cancellationToken);
			}
			catch (Exception ex) {
				_logger.LogError(ex, "An error occured checking for updates.");
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public override async Task StopAsync(CancellationToken cancellationToken) {
			cancellationToken.Register(() => _cancellationSource.Cancel(false));
			await base.StopAsync(cancellationToken);
		}
	}

	public partial class UpdaterService {
		public static void AddServices(IServiceCollection services, HostBuilderContext host) {
			//var path = host.Configuration.GetValue<string>(AppSettingsReleasesConfigKey);
			//var cronTime = host.Configuration.GetValue<string>(AppSettingsUpdateScheduleKey);
			services.AddHostedService(c => ActivatorUtilities.CreateInstance<UpdaterService>(c, CronSchedules.Hourly));
		}
	}
}