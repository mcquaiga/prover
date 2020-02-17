using System;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Core;

namespace Devices.Honeywell.Comm
{
    public class HoneywellClientFactory : IDeviceTypeCommClientFactory<HoneywellDeviceType>
    {
        #region ICommClientFactory<HoneywellDeviceType> Members

        public ICommunicationsClient Create(HoneywellDeviceType deviceType, ICommPort commPort
          )
        {
            var client = new HoneywellClient(commPort, deviceType);

            //if (statusObserver != null)
            //    client.StatusMessages.Subscribe(statusObserver);

            //await client.ConnectAsync(retryAttempts, timeout);

            return client;
        }

        #endregion
    }
}