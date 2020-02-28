using Devices.Communications.IO;
using Devices.Honeywell.Core;
using Prover.Shared.IO;

namespace Devices.Honeywell.Comm.CommClients
{
    public class HoneywellClient : HoneywellClientBase<HoneywellDeviceType>
    {
        public HoneywellClient(ICommPort commPort, HoneywellDeviceType deviceType) : base(commPort, deviceType)
        {
        }
    }
}