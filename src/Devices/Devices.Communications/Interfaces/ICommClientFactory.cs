using Devices.Communications.IO;
using Devices.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Devices.Communications.Interfaces
{
    public interface ICommClientFactory<T, TInstance>
        where T : DeviceType
        where TInstance : DeviceInstance
    {
        Task<ICommunicationsClient<T, TInstance>> Create(T deviceType, ICommPort commPort, int retryAttempts = 1, TimeSpan? timeout = null, IObserver<string> statusObserver = null);
    }
}