using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;

namespace Prover.Application.Interfaces
{
    public enum VerificationTestStep
    {
        OnInitialize,
        OnSubmit,
        OnVolumeStart,
        OnVolumeEnd
    }

    public interface IVerificationCustomActions
    {
        VerificationTestStep RunOnStep { get; }
        Task<bool> Run(IDeviceSessionManager deviceManager, DeviceInstance device, EvcVerificationViewModel verification);
    }
}