using Devices.Communications;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Devices.Honeywell.Comm
{
    public class HoneywellClientFactory : ICommClientFactory<IHoneywellDeviceType>
    {
        public async Task<IEvcCommunicationClient<IHoneywellDeviceType>> Create(IHoneywellDeviceType deviceType, ICommPort commPort, int retryAttempts = 1, TimeSpan? timeout = null)
        {
            var client = new HoneywellClient(commPort);

            await client.ConnectAsync(deviceType, retryAttempts, timeout);

            await client.GetAllItems();

            return client;
        }
    }
}