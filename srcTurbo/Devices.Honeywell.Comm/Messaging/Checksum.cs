using Honeywell.Metering.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Honeywell.Comm.Messaging
{
    public static class Checksum
    {
        public static string CalcCRC(string body)
        {
            return Utilities.CalcCRC(body);
        }
    }
}