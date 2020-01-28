using Honeywell.Metering.Utils;

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