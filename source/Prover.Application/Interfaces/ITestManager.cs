using System.Threading.Tasks;
using Prover.Application.ViewModels;

namespace Prover.Application.Interfaces
{
    public interface ITestManager
    {
        EvcVerificationViewModel TestViewModel { get; }
        IVolumeTestManager VolumeTestManager { get; }
        Task RunCorrectionTests(VerificationTestPointViewModel test);
    }
}