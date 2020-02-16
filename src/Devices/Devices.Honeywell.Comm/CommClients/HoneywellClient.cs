using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Honeywell.Core;

namespace Devices.Honeywell.Comm.CommClients
{
    public class HoneywellClient : HoneywellClientBase<HoneywellDeviceType>
    {
        public HoneywellClient(ICommPort commPort, HoneywellDeviceType deviceType) : base(commPort, deviceType)
        {
        }
    }
}