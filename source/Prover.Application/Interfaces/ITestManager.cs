using Prover.Application.Verifications;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
	public interface IQaTestRunManager : IRoutableViewModel
	{
		IDeviceSessionManager DeviceManager { get; }
		EvcVerificationViewModel TestViewModel { get; }
	}

	public interface ITestManagersProvider
	{
		IVolumeTestManager VolumeTestManager { get; }
		ICorrectionTestsManager CorrectionVerifications { get; }
	}
}