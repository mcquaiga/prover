using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Core;
using System;
using System.Threading.Tasks;

namespace Devices.Honeywell.Comm
{
    public class HoneywellClientFactory : ICommClientFactory<HoneywellDeviceType>
    {
        public async Task<ICommunicationsClient> Create(HoneywellDeviceType deviceType, ICommPort commPort, int retryAttempts = 1, TimeSpan? timeout = null)
        {
            var client = new HoneywellClient(commPort, deviceType);

            await client.ConnectAsync(retryAttempts, timeout);

            return client;
        }
    }
}