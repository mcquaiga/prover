using Devices.Core.Interfaces;
using System.Threading.Tasks;

namespace Prover.Application.Interfaces
{
    public interface IVerificationManagerFactory
    {
        Task<IQaTestRunManager> StartNew(DeviceType deviceType);
    }
}