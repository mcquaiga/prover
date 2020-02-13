using System;
using Devices.Communications.IO;
using Devices.Romet.Core;

namespace Devices.Romet.Comm
{
    public class RometClientFactory
    {
        public static RometClient Create(RometDeviceType deviceType, ICommPort commPort, int retryAttempts = 1, TimeSpan? timeout = null)
        {
            return new RometClient(commPort, deviceType);
        }
    }
}
