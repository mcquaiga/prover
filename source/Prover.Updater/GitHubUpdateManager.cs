using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octokit;
using Prover.Application;
using Prover.Shared;
using Prover.Shared.Extensions;
using FileMode = System.IO.FileMode;

namespace Prover.Updater {
	public enum HealthStatus {
		OK,
		Critical,
		Warning
	}

	public interface IUpdateManager {
		HealthStatus Status { get; }
		Task<bool> CheckForUpdate();
		Task<Version> GetLatestVersion();
		Task Update(CancellationToken cancellationToken);
	}

	public class UpdateManagerBase : IUpdateManager {
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

		public Task<Version> GetLatestVersion() {
			return Task.FromResult(ProverApp.GetVersion());
		}
		// 

		public virtual async Task Update(CancellationToken cancellationToken) {
			if (GetStatus() == HealthStatus.Critical)
				return;

			_logger.LogInformation($"Checking GitHub for new release.");

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

	public class GitHubUpdateManager : UpdateManagerBase {

		private const string _token = "ba4ca40bc3d608a43c7d7f3d790d5e3c63073c51";
		private readonly string _releasePath = "https://api.github.com/repos/mcquaiga/EvcProver/releases/latest";

		//private GitHubClient _githubClient;
		private readonly long _repoId = 21523845;


		protected GitHubUpdateManager(string updaterPath, string releasePath) {
			_releasePath = Validate.NullOrEmpty(releasePath);
			_updateExePath = Validate.NullOrEmpty(updaterPath);
		}

		public GitHubUpdateManager() : base() {
			_updateExePath = GetDefaultUpdateExePath().PathCombine(_updateExePath);
			_downloadPath = GetDefaultUpdateExePath().PathCombine(_downloadPath);
			_process = new ProcessStartInfo(_updateExePath);
		}

		/// <inheritdoc />
		public override async Task<bool> CheckForUpdate() => CheckForUpdate(await GetLatestRelease());

		/// <inheritdoc />
		public async Task DownloadLatestAssets(string outputDir, CancellationToken cancellationToken) {
			var release = await GetLatestRelease();
			await DownloadAssets(release, outputDir);
		}

		/// <inheritdoc />
		public override async Task Update(CancellationToken cancellationToken) {
			LoggerExtensions.LogInformation(_logger, $"Checking GitHub for new release.");
			var release = await GetLatestRelease();

			if (CheckForUpdate(release)) {
				LoggerExtensions.LogInformation(_logger, $"Found update with ver. {release.TagName}");
				var releaseUrl = await DownloadAssets(release); //await GetLatestVersionUrl();
				StartUpdate(releaseUrl);
			}
		}

		private bool CheckForUpdate(Release newRelease) {
			var current = ProverApp.GetVersion();
			var update = Version.Parse(newRelease.TagName);
			return !current.Equals(update);
		}

		private async Task<string> DownloadAssets(Release release, string outputDir = null) {
			outputDir = outputDir ?? _downloadPath;

			_logger.LogDebug($"Getting release assets for ver. {release.TagName}...");
			var client = GetGitHubClient();
			foreach (var asset in release.Assets.Where(a => !a.Name.Contains("Setup.exe"))) {
				_logger.LogDebug($"  Downloading { asset.Name } ...");
				var path = FileDownloadPath(asset);

				var responseRaw = await GetRawAssetResponse(asset);
				await WriteResponseStreamToFile(path, responseRaw);
			}

			return outputDir;

			string FileDownloadPath(ReleaseAsset asset) {
				Directory.CreateDirectory(outputDir);

				var path = outputDir.PathCombine(asset.Name);

				if (File.Exists(path))
					File.Delete(path);
				return path;
			}

			async Task<IApiResponse<byte[]>> GetRawAssetResponse(ReleaseAsset asset) {
				var responseRaw = await client.Connection.Get<byte[]>(new Uri(asset.Url), new Dictionary<string, string>(), "application/octet-stream");
				return responseRaw;
			}

			async Task WriteResponseStreamToFile(string path, IApiResponse<byte[]> responseRaw) {

				using (var reader = new StreamContent(new MemoryStream(responseRaw.Body))) {
					using (var writer = new FileStream(path, FileMode.Create)) {
						await reader.CopyToAsync(writer);
					}
				}
			}
		}

		private WebClient GetWebClient() {
			var webClient = new WebClient();
			webClient.Headers.Add("Authorization", $"token {_token}");
			webClient.Headers.Add("Accept", "application/vnd.github.v3.raw");
			return webClient;
		}

		private GitHubClient GetGitHubClient() {
			var product = new ProductHeaderValue("evcprover-updater");

			//var conn = new Connection(product);
			var client = new GitHubClient(product);
			client.Credentials = new Credentials(_token);
			return client;
		}

		private Task<Release> GetLatestRelease() {
			var client = GetGitHubClient();
			return client.Repository.Release.GetLatest(_repoId);
		}

		//public async Task<string> GetVersion() {
		//	var release = await GetLatestRelease();
		//	return release.TagName;
		//}

		private async Task<string> GetLatestVersionUrl() {
			var releaseInfo = await GetLatestRelease();
			return releaseInfo.HtmlUrl.Replace("tag", "download");
		}




	}
}