using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Updater;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Updater.Tests {
	[TestClass()]
	public class UpdaterServiceTests {
		[TestMethod()]
		public async Task UpdaterServiceTest() {
			var updater = new GitHubUpdateManager();

			await updater.Update(new CancellationToken());
		}

		[TestMethod()]
		public void AddServicesTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void DoWorkTest() {
			Assert.Fail();
		}
	}
}