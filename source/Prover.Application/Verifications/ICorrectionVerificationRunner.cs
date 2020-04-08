using System.Threading.Tasks;
using Prover.Application.ViewModels;

namespace Prover.Application.Verifications
{
    public interface ICorrectionVerificationRunner
    {
        Task RunCorrectionTests(VerificationTestPointViewModel test);
    }
}