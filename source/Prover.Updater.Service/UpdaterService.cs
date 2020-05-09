using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Shared;
using Prover.UI.Desktop.Common;
using Squirrel;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Updater
{
	public partial class UpdaterService : ServiceBase, IHostedService
	{
		public UpdaterService()
		{
			InitializeComponent();
		}

		/// <inheritdoc />
		public Task StartAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

		/// <inheritdoc />
		public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

		protected override void OnStart(string[] args)
		{
		}

		protected override void OnStop()
		{
		}
	}

	public class Updater : CronJobService, IConfigureModule
	{
		private const string AppSettingsReleasesConfigKey = "Releases:Path";
		private const string AppSettingsUpdateScheduleKey = "Releases:UpdateSchedule";
		private readonly ILogger<Updater> _logger;
		private readonly string _releasePath;

		public Updater(ILogger<Updater> logger, string releasePath, string cronExpression, TimeZoneInfo timeZoneInfo) : base(cronExpression, timeZoneInfo)
		{
			_logger = logger;
			_releasePath = releasePath;


		}

		public override async Task DoWork(CancellationToken cancellationToken)
		{
			try
			{
				using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/mcquaiga/EvcProver"))
				{
					var updateInfo = await mgr.CheckForUpdate();
					await mgr.UpdateApp();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occured");
				await StopAsync(cancellationToken);
				Dispose();
			}
		}

		private async Task RunUpdate(CancellationToken cancellationToken)
		{
			await Task.CompletedTask;


		}

		/// <inheritdoc />
		public void ConfigureServices(HostBuilderContext builder, IServiceCollection services)
		{
			//var path = host.Configuration.GetValue<string>(AppSettingsReleasesConfigKey);
			//var cronTime = host.Configuration.GetValue<string>(AppSettingsUpdateScheduleKey);
			var path = Environment.ExpandEnvironmentVariables("%localappdata%\\Releases");
			var cronTime = "0 0 * * *";

			if (string.IsNullOrEmpty(path))
				return;

			services.AddHostedService(c => ActivatorUtilities.CreateInstance<Updater>(c, path, cronTime, TimeZoneInfo.Local));

			//services.AddSingleton<ILogManager, ProverLogManager>();
			//services.AddSingleton<ILogger<UpdateManager>>(c => )
		}

		/// <inheritdoc />


		/// <inheritdoc />
		public void ConfigureAppConfiguration(HostBuilderContext builder, IConfigurationBuilder config)
		{

		}
	}
}

//var splatLogger = new RxLogging(_logger);
//var splatLog = new WrappingFullLogger(splatLogger);
//var logManager = Locator.Current.GetService<ILogManager>((string)null);

//var sLog = logManager.GetLogger<UpdateManager>();

//Locator.CurrentMutable.Register(() => splatLog, typeof(Splat.ILogger));
//
/// //var updateInfo = await mgr.CheckForUpdate();
//if (updateInfo.ReleasesToApply.Any())
//{
//    await mgr.UpdateApp();
//    Messages.ShowMessage.Handle("Update applied.");
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
//}