using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Core;
using System;
using System.Threading.Tasks;

namespace Devices.Honeywell.Comm
{
    public class HoneywellClientFactory : ICommClientFactory<HoneywellDeviceType, HoneywellDeviceInstance>
    {
        public async Task<ICommunicationsClient<HoneywellDeviceType, HoneywellDeviceInstance>> Create(HoneywellDeviceType deviceType, ICommPort commPort, int retryAttempts = 1, TimeSpan? timeout = null,
            IObserver<string> statusObserver = null)
        {
            var client = new HoneywellClient(commPort, deviceType);

            if (statusObserver != null)
                client.StatusMessages.Subscribe(statusObserver);

            await client.ConnectAsync(retryAttempts, timeout);

            return client;
        }
    }
}