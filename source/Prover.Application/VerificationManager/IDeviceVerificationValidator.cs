using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;

namespace Prover.Application.VerificationManager
{
    public interface IVerificationActionsExecutioner
    {
        Task RunCustomActions(VerificationTestStep testStep, EvcVerificationViewModel verificationTest, DeviceInstance device);
    }
}