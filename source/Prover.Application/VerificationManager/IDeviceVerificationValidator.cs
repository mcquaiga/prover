using System.Threading.Tasks;
using Devices.Core.Interfaces;

namespace Prover.Application.VerificationManager
{
    public interface IDeviceVerificationValidator
    {
        Task RunValidations(DeviceInstance device);
    }
}