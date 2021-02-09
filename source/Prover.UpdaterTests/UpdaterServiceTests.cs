using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Updater;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Updater.Tests {

	//[TestClass()]
	public class UpdaterServiceTests {
		private static GitHubUpdateManager _updater = new GitHubUpdateManager();

		[TestMethod()]
		public async Task CheckForUpdateTest() {

			Assert.IsTrue(await _updater.CheckForUpdate());
		}

		[TestMethod()]
		public async Task GetLatestReleaseTest() {
			var ver = await _updater.GetLatestVersion();
			Assert.IsTrue(ver != null);
		}

		[TestMethod()]
		public async Task DownloadAssets() {
			await _updater.DownloadLatestAssets(".\\downloads", new CancellationToken());
		}
	}
}