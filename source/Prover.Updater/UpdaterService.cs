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
	public class UpdaterService : CronJobService {
		private const string AppSettingsReleasesConfigKey = "Releases:Path";
		private const string AppSettingsUpdateScheduleKey = "Releases:UpdateSchedule";
		private readonly ILogger<UpdaterService> _logger;

		private readonly GitHubUpdateManager _gitHubUpdateManager;

		public UpdaterService(ILogger<UpdaterService> logger, IHostApplicationLifetime host, CronExpression schedule) : base(schedule) {

			_logger = logger ?? NullLogger<UpdaterService>.Instance;
			_gitHubUpdateManager = new GitHubUpdateManager();

			//host.ApplicationStarted.Register(() => {
			//	Task.Run(() => CheckForUpdate(new CancellationToken()));
			//}, true);
		}

		public static void AddServices(IServiceCollection services, HostBuilderContext host) {

			//var path = host.Configuration.GetValue<string>(AppSettingsReleasesConfigKey);
			var cronTime = host.Configuration.GetValue<string>(AppSettingsUpdateScheduleKey);

			services.AddHostedService(c => ActivatorUtilities.CreateInstance<UpdaterService>(c, CronSchedules.Hourly));
		}

		public override async Task DoWork(CancellationToken cancellationToken) {

			if (_gitHubUpdateManager.Status == HealthStatus.Critical) {
				await StopAsync(cancellationToken);
				return;
			}

			await CheckForUpdate(cancellationToken);
		}



		public Task CheckForUpdate(CancellationToken cancellationToken) {

			//cancellationToken = cancellationToken ??

			try {
				_logger.LogInformation("Checking for updates....");
				return _gitHubUpdateManager.Update(cancellationToken);
			}
			catch (Exception ex) {
				_logger.LogError(ex, "An error occured checking for updates.");
			}

			return Task.CompletedTask;
		}



		//private async Task CheckForUpdate(CancellationToken cancellationToken)
		//{

		//    //using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/mcquaiga/EvcProver"))
		//    //{
		//    //    await mgr.CheckForUpdate();

		//    //    await mgr.UpdateApp();
		//    //}

		//    //var updateInfo = await mgr.CheckForUpdate();
		//    //if (updateInfo.ReleasesToApply.Any())
		//    //{
		//    //    await mgr.UpdateApp();
		//    //    Messages.ShowMessage.Handle("Update applied.");
		//    //}
		//}

		//private async Task<IUpdateManager> GetUpdateManager()
		//{
		//    await Task.CompletedTask;

		//    UpdateManager mgr;

		//    if (_releasePath.Contains("github"))
		//    {
		//        _logger.Log(LogLevel.Information, $"Checking for updates on GitHub");
		//        mgr = await UpdateManager.GitHubUpdateManager(_releasePath);
		//    }
		//    else
		//    {
		//        _logger.Log(LogLevel.Information, $"Checking for updates at {_releasePath}");
		//        mgr = new UpdateManager(_releasePath);
		//    }

		//    return mgr;
	}
}