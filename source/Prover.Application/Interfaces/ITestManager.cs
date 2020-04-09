using System.Threading.Tasks;
using Prover.Application.VerificationManagers;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
    public interface ITestManager : IReactiveObject
    {
        EvcVerificationViewModel TestViewModel { get; }
        IVolumeTestManager VolumeTestManager { get; }
        ICorrectionTestsManager CorrectionVerifications { get; }
    }
}