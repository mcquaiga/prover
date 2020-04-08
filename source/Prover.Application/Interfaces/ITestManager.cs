using System.Threading.Tasks;
using Prover.Application.Verifications;
using Prover.Application.ViewModels;

namespace Prover.Application.Interfaces
{
    public interface ITestManager
    {
        EvcVerificationViewModel TestViewModel { get; }
        IVolumeTestManager VolumeTestManager { get; }
        ICorrectionVerificationRunner CorrectionVerifications { get; }
    }
}