using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Core;
using Devices.Romet.Core;

namespace Devices.Romet.Comm
{
    public class RometClientFactory
    {
        public static async Task<RometClient> CreateAsync(
            RometDeviceType deviceType, ICommPort commPort, int retryAttempts = 1, TimeSpan? timeout = null,
            IObserver<string> statusObserver = null)
        {
            return await new RometClientFactory().Create(deviceType, commPort, retryAttempts, timeout, statusObserver);
        }

        public async Task<RometClient> Create(RometDeviceType deviceType, ICommPort commPort, int retryAttempts = 1, TimeSpan? timeout = null,
            IObserver<string> statusObserver = null)
        {
            var client = new RometClient(commPort, deviceType);

            if (statusObserver != null)
                client.StatusMessages.Subscribe(statusObserver);

            client.StatusMessages.Subscribe(Console.WriteLine);
            client.CommunicationMessages
                .Subscribe(Console.WriteLine);

            
            return client;
        }
    }
}
