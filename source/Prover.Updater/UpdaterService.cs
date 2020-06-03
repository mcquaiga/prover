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
using Prover.Application.Interactions;
using Prover.UI.Desktop.Common;

namespace Prover.Updater {

	public static class UpdaterServiceEx {
		private const string AutoUpdateConfigKey = "AutoUpdate";

		public static void UpdateService(this IServiceCollection services, HostBuilderContext host) {
#if (!DEBUG)
			var runUpdater = host.Configuration.GetValue<bool>(AutoUpdateConfigKey);
			//var cronTime = host.Configuration.GetValue<string>(AppSettingsUpdateScheduleKey);

			if (runUpdater)
				services.AddHostedService(c => ActivatorUtilities.CreateInstance<UpdaterService>(c, CronSchedules.Hourly));
#endif
		}
	}

	public partial class UpdaterService : CronJobService {
		//private const string AppSettingsUpdateScheduleKey = "Releases:UpdateSchedule";
		private readonly ILogger<UpdaterService> _logger;
		private CancellationTokenSource _cancellationSource = new CancellationTokenSource();
		private readonly GitHubUpdateManager _updateManager;

		public UpdaterService(ILogger<UpdaterService> logger, IHostApplicationLifetime host, CronExpression schedule) : base(schedule) {
			_logger = logger ?? NullLogger<UpdaterService>.Instance;
			_updateManager = new GitHubUpdateManager();

			host.ApplicationStarted.Register(() => {
				//Task.Run(() => DoWork(_cancellationSource.Token));
			}, true);
		}

		public override async Task DoWork(CancellationToken cancellationToken) {
			if (_updateManager.Status == HealthStatus.Critical) {
				await StopAsync(cancellationToken);
			}

			_logger.LogInformation("Checking for updates....");
			try {

				if (await _updateManager.CheckForUpdate()) {

					await _updateManager.Update(cancellationToken);

					await NotifyUpdated();

					_logger.LogInformation("Update finished!");
				}
			}
			catch (Exception ex) {
				_logger.LogError(ex, "An error occured checking for updates.");
			}
		}

		public async Task NotifyUpdated() {
			await Notifications.PersistentMessage.Handle($"New update installed. EVC Prover requires restart.");
		}

		/// <inheritdoc />
		public override async Task StopAsync(CancellationToken cancellationToken) {
			cancellationToken.Register(() => _cancellationSource.Cancel(false));
			await base.StopAsync(cancellationToken);
		}
	}


}