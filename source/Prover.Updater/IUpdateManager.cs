using System;
using System.Threading;
using System.Threading.Tasks;

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
}