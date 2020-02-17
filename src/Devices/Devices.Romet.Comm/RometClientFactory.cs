using System;
using System.Threading.Tasks;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Romet.Core;

namespace Devices.Romet.Comm
{
    public class RometClientFactory : IDeviceTypeCommClientFactory<RometDeviceType>
    {
        public ICommunicationsClient Create(RometDeviceType deviceType, ICommPort commPort)
        {
            var client = new RometClient(commPort, deviceType);

            //await client.ConnectAsync(retryAttempts, timeout);

            return client;
        }
    }
}
