using System;
using System.Threading.Tasks;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Romet.Core;

namespace Devices.Romet.Comm
{
    public class RometClientFactory : ICommClientFactory<RometDeviceType>
    {
        public async Task<ICommunicationsClient> Create(RometDeviceType deviceType, ICommPort commPort, int retryAttempts = 1, TimeSpan? timeout = null,
            IObserver<string> statusObserver = null)
        {
            var client = new RometClient(commPort, deviceType);

            if (statusObserver != null)
                client.StatusMessages.Subscribe(statusObserver);

            await client.ConnectAsync(retryAttempts, timeout);

            return client;
        }
    }
}
