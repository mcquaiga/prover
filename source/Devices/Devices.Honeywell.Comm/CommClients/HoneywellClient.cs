using Devices.Honeywell.Core;
using Prover.Shared.Interfaces;

namespace Devices.Honeywell.Comm.CommClients
{
    public class HoneywellClient : HoneywellClientBase<HoneywellDeviceType>
    {
        public HoneywellClient(ICommPort commPort, HoneywellDeviceType deviceType) : base(commPort, deviceType)
        {
        }
    }
}