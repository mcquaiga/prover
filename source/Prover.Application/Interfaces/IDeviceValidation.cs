using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Domain.EvcVerifications;

namespace Prover.Application.Interfaces
{
    public interface IDeviceValidation
    {
        Task<bool> Validate(IDeviceSessionManager deviceManager, DeviceInstance device);
    }
}