using System.Threading.Tasks;
using Prover.Application.ViewModels;

namespace Prover.Application.Interfaces
{
    public interface ICorrectionTestsManager
    {
        Task RunCorrectionTests(VerificationTestPointViewModel test);
    }
}