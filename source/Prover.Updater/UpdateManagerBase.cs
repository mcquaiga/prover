using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prover.Application;
using Prover.Shared;

namespace Prover.Updater {
	public abstract class UpdateManagerBase : IUpdateManager {
		protected readonly ILogger<UpdateManagerBase> _logger = ProverLogging.CreateLogger<UpdateManagerBase>();
		protected string _updateExePath = "Update.exe";
		protected string _downloadPath = "downloads";
		protected ProcessStartInfo _process;

		public UpdateManagerBase() {

			_updateExePath = Extensions.PathCombine((string)GetDefaultUpdateExePath(), _updateExePath);
			_downloadPath = Extensions.PathCombine((string)GetDefaultUpdateExePath(), _downloadPath);
			_process = new ProcessStartInfo(_updateExePath);
		}

		public HealthStatus Status => GetStatus();

		public virtual async Task<bool> CheckForUpdate() {
			var latest = await GetLatestVersion();
			return !ProverApp.GetVersion().Equals(latest);
		}

		public abstract Task<Version> GetLatestVersion();
		//{
		//	return Task.FromResult(ProverApp.GetVersion());
		//}
		// 

		public virtual async Task Update(CancellationToken cancellationToken) {
			if (GetStatus() == HealthStatus.Critical)
				return;

			_logger.LogInformation($"Checking for new release.");

			if (await CheckForUpdate()) {
				_logger.LogInformation($"Found update with ver.");

				StartUpdate(_downloadPath);
			}
		}

		protected string GetDefaultUpdateExePath() {
			var basePath = Directory.GetParent(Assembly.GetEntryAssembly().Location).Parent;
			return basePath.FullName;
		}

		protected virtual void StartUpdate(string releaseUrl) {

			_logger.LogDebug($"Starting update process ...");
			_process = _process ?? new ProcessStartInfo(_updateExePath);
			_process.Arguments = $"--update={releaseUrl}";
			_process.UseShellExecute = true;
			//Task.Run(() => Process.Start(_process));
			var proc = Process.Start(_process);
			proc.WaitForExit();

			_logger.LogDebug($"Update.exe process finished exit code = {proc.ExitCode}.");
		}
		protected bool UpdateExeExists(string path) => File.Exists(_updateExePath);

		protected virtual HealthStatus GetStatus() {
			var status = HealthStatus.OK;

			if (!UpdateExeExists(_updateExePath))
				status = HealthStatus.Critical;
			return status;
		}
	}
}