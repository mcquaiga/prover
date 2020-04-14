using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.Services;

namespace Prover.Application.Interfaces
{
    public interface IVerificationManagerFactory
    {
        Task<ITestManager> StartNew(DeviceType deviceType);
    }
}