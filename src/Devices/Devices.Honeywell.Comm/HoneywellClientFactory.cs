using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Core;
using Prover.Shared.IO;

namespace Devices.Honeywell.Comm
{
    public class HoneywellClientFactory : IDeviceTypeCommClientFactory<HoneywellDeviceType>
    {
        #region ICommClientFactory<HoneywellDeviceType> Members

        public ICommunicationsClient Create(HoneywellDeviceType deviceType, ICommPort commPort
          )
        {
            var client = new HoneywellClient(commPort, deviceType);

            return client;
        }

        #endregion
    }
}