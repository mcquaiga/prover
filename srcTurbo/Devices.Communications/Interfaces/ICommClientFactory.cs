using Devices.Communications.IO;
using Devices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Devices.Communications.Interfaces
{
    public interface ICommClientFactory<in T>
        where T : IDeviceType
    {
        Task<ICommunicationsClient> Create(T deviceType, ICommPort commPort, int retryAttempts = 1, TimeSpan? timeout = null, IObserver<string> statusObserver = null);
    }
}