using Devices.Communications.IO;
using Devices.Honeywell.Comm;
using Devices.Honeywell.Core;

namespace Devices.Honeywell.Tests.CommTests.Messaging
{
    internal static class Messages
    {
        internal static class Incoming
        {
            public static string SignOnErrorResponse => BuildResponse(ResponseCode.SignOnError, "F053");

            public static string SuccessResponse => BuildResponse(ResponseCode.NoError, "F053");

            public static string GetResponse(ResponseCode code)
                => BuildResponse(code, "0000");

            private static string BuildResponse(ResponseCode responseCode, string checksum)
            {
                var code = ((int)responseCode).ToString().PadLeft(2, '0');
                return $"{ControlCharacters.SOH}{code}{ControlCharacters.ETX}{checksum}{ControlCharacters.EOT}";
            }
        }

        internal static class Outgoing
        {
            public static string BadSignOnMessage(HoneywellDeviceType type)
                            => $"{ControlCharacters.SOH}SN,33333{ControlCharacters.STX}vq{type.AccessCode.ToString().PadLeft(2, '0')}{ControlCharacters.ETX}0000{ControlCharacters.EOT}";

            public static string SignOnMessage(HoneywellDeviceType type)
                            => $"{ControlCharacters.SOH}SN,33333{ControlCharacters.STX}vq{type.AccessCode.ToString().PadLeft(2, '0')}{ControlCharacters.ETX}415D{ControlCharacters.EOT}";
        }
    }
}