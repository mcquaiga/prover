using System;
using System.Collections.Generic;
using System.Text;
using Devices.Communications.IO;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Core;
using Devices.Romet.Core;

namespace Devices.Romet.Comm
{
    public class RometClient : HoneywellClient
    {
        public RometClient(ICommPort commPort, RometDeviceType deviceType) : base(commPort, deviceType)
        {
        }
    }
}
