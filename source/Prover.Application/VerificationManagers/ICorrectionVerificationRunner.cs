using System.Threading.Tasks;
using Prover.Application.ViewModels;

namespace Prover.Application.VerificationManagers
{
    public interface ICorrectionTestsManager
    {
        Task RunCorrectionTests(VerificationTestPointViewModel test);
    }
}