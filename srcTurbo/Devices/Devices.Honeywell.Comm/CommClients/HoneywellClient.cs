using System;
using System.Threading;
using System.Threading.Tasks;
using Devices.Communications.IO;
using Devices.Core.Items;
using Devices.Honeywell.Core;

namespace Devices.Honeywell.Comm.CommClients
{
    public class HoneywellClient : HoneywellClientBase<HoneywellDeviceType, HoneywellDeviceInstance>
    {
        public HoneywellClient(ICommPort commPort, HoneywellDeviceType deviceType) : base(commPort, deviceType)
        {
        }
    }
}