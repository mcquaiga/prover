using Devices.Communications.IO;
using Devices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Devices.Communications.Interfaces
{
    public interface ICommClientFactory<TDevice>
        where TDevice : IDeviceType
    {
        Task<IEvcCommunicationClient<TDevice>> Create(TDevice deviceType, ICommPort commPort, int retryAttempts = 1, TimeSpan? timeout = null);
    }
}