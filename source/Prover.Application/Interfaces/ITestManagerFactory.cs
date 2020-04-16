using System.Threading.Tasks;
using Devices.Core.Interfaces;

namespace Prover.Application.Interfaces
{
    public interface IVerificationManagerFactory
    {
        Task<IDeviceQaTestManager> StartNew(DeviceType deviceType);
    }
}