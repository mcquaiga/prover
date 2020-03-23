using System.Threading.Tasks;
using Devices.Core.Interfaces;

namespace Prover.Application.Interfaces
{
    public interface ITestManagerFactory
    {
        Task<ITestManager> StartNew(DeviceType deviceType, string commPortName, int baudRate,
            string tachPortName);

    }
}