using Devices.Communications.IO;
using Devices.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Devices.Communications.Interfaces
{
    public interface ICommClientFactory<in TDevice>
        where TDevice : DeviceType
    {
    Task<ICommunicationsClient> Create(TDevice deviceType, ICommPort commPort, int retryAttempts = 1,
        TimeSpan? timeout = null, IObserver<string> statusObserver = null);
    }
}