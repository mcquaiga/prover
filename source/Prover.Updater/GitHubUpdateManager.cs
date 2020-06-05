using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octokit;
using Prover.Shared;
using Prover.Shared.Extensions;
using FileMode = System.IO.FileMode;

namespace Prover.Updater {
	public class GitHubUpdateManager : UpdateManagerBase {
		private const string _token = "ba4ca40bc3d608a43c7d7f3d790d5e3c63073c51";
		private readonly string _releasePath = "https://api.github.com/repos/mcquaiga/EvcProver/releases/latest";

		//private GitHubClient _githubClient;
		private readonly long _repoId = 21523845;


		protected GitHubUpdateManager(string updaterPath, string releasePath) {
			_releasePath = Validate.NullOrEmpty(releasePath);
			_updateExePath = Validate.NullOrEmpty(updaterPath);
		}

		public GitHubUpdateManager() {
			_updateExePath = GetDefaultUpdateExePath().PathCombine(_updateExePath);
			_downloadPath = GetDefaultUpdateExePath().PathCombine(_downloadPath);
			_process = new ProcessStartInfo(_updateExePath);
		}

		/// <inheritdoc />
		public override async Task<bool> CheckForUpdate() {
			return IsNewerVersion(await GetLatestRelease());
		}

		/// <inheritdoc />
		public async Task DownloadLatestAssets(string outputDir, CancellationToken cancellationToken) {
			var release = await GetLatestRelease();
			await DownloadAssets(release, outputDir);
		}

		/// <inheritdoc />
		public override async Task Update(CancellationToken cancellationToken) {
			_logger.LogInformation("Checking GitHub for new release.");
			var release = await GetLatestRelease();

			if (IsNewerVersion(release)) {
				_logger.LogInformation($"Found update with ver. {release.TagName}");
				var releaseUrl = await DownloadAssets(release); //await GetLatestVersionUrl();
				StartUpdate(releaseUrl);
			}
		}

		/// <inheritdoc />
		public override async Task<Version> GetLatestVersion() {
			var rel = await GetLatestRelease();
			return Version.Parse(rel.TagName);
		}

		private async Task<string> DownloadAssets(Release release, string outputDir = null) {
			outputDir = outputDir ?? _downloadPath;
			_logger.LogDebug($"Getting release assets for ver. {release.TagName}...");
			var client = GetGitHubClient();

			foreach (var asset in release.Assets.Where(a => !a.Name.Contains("Setup.exe"))) {
				_logger.LogDebug($"  Downloading {asset.Name} ...");
				var path = FileDownloadPath(asset, outputDir);
				var responseRaw = await GetRawAssetResponse(asset);
				await WriteResponseStreamToFile(path, responseRaw);
			}

			return outputDir;

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

		private string FileDownloadPath(ReleaseAsset asset, string outputDir) {
			Directory.CreateDirectory(outputDir);
			var path = outputDir.PathCombine(asset.Name);

			if (File.Exists(path))
				File.Delete(path);
			return path;
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

		private WebClient GetWebClient() {
			var webClient = new WebClient();
			webClient.Headers.Add("Authorization", $"token {_token}");
			webClient.Headers.Add("Accept", "application/vnd.github.v3.raw");
			return webClient;
		}

		private bool IsNewerVersion(Release newRelease) {
			var current = ProverApp.GetVersion();
			var update = Version.Parse(newRelease.TagName);
			return !current.Equals(update);
		}
	}
}