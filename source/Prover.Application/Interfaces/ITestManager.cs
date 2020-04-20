using Prover.Application.Verifications;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
    public interface IQaTestRunManager : IReactiveObject
    {
        EvcVerificationViewModel TestViewModel { get; }
    }

    public interface IDeviceQaTestManager : IQaTestRunManager
    {
        IDeviceSessionManager DeviceManager { get; }
        IVolumeTestManager VolumeTestManager { get; }
        ICorrectionTestsManager CorrectionVerifications { get; }
    }
}