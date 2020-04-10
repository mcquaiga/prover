using System.Threading.Tasks;
using Prover.Application.ViewModels;

namespace Prover.Application.Verifications
{
    public interface ICorrectionTestsManager
    {
        Task RunCorrectionTests(VerificationTestPointViewModel test);
    }
}