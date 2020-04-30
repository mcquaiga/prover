using Devices.Communications.Interfaces;
using Devices.Romet.Core;
using Prover.Shared.Interfaces;

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
