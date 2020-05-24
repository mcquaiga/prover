using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Octokit;
using Prover.Shared.Extensions;

namespace Prover.Updater {
	public enum HealthStatus {
		OK,
		Critical,
		Warning
	}

	public class GitHubUpdateManager {
		private const string _token = "ba4ca40bc3d608a43c7d7f3d790d5e3c63073c51";
		private readonly string _releasePath = "https://api.github.com/repos/mcquaiga/EvcProver/releases/latest";
		private readonly string _updateExe = "Update.exe";

		private readonly string _updaterPath;

		//private GitHubClient _githubClient;
		private readonly long _repoId = 21523845;
		private ProcessStartInfo _process;

		protected GitHubUpdateManager(string updaterPath, string releasePath) {
			_releasePath = Validate.NullOrEmpty(releasePath);
			_updaterPath = Validate.NullOrEmpty(updaterPath);
		}

		public GitHubUpdateManager() {
			_updaterPath = GetDefaultUpdateExePath().PathCombine(_updateExe);
			_process = new ProcessStartInfo(_updaterPath);
		}

		public HealthStatus Status => GetStatus();

		private HealthStatus GetStatus() {
			var status = HealthStatus.OK;

			if (!UpdateExeExists(_updaterPath))
				status = HealthStatus.Critical;
			return status;
		}

		public async Task Update(CancellationToken cancellationToken) {
			if (GetStatus() == HealthStatus.Critical)
				return;

			var releaseUrl = await GetLatestVersionUrl();

			_process = _process ?? new ProcessStartInfo(_updaterPath);
			_process.Arguments = $"--update={releaseUrl}";
			Process.Start(_process);
		}

		private string GetDefaultUpdateExePath() {
			var basePath = Directory.GetParent(Assembly.GetEntryAssembly().Location).Parent;

			return basePath.FullName;
		}

		private GitHubClient GetGitHubClient() {
			var client = new GitHubClient(new ProductHeaderValue("evcprover-updater"));
			client.Credentials = new Credentials(_token);
			return client;
		}

		private Task<Release> GetLatestRelease(GitHubClient client) => client.Repository.Release.GetLatest(_repoId);

		private async Task<string> GetLatestVersionUrl() {
			var client = GetGitHubClient();
			var releaseInfo = await GetLatestRelease(client);
			return releaseInfo.HtmlUrl.Replace("tag", "download");
		}

		private bool UpdateExeExists(string path) => File.Exists(_updaterPath);
	}
}