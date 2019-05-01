using Devices.Communications.IO;
using Devices.Honeywell.Comm;
using Devices.Honeywell.Core;

namespace Tests.Devices.Honeywell.Comm.Messaging
{
    internal static class Messages
    {
        #region Classes

        internal static class Incoming
        {
            #region Properties

            public static string SuccessResponse => BuildResponse(ResponseCode.NoError, "F053");

            public static string GetResponse(ResponseCode code)
                => BuildResponse(code, "0000");

            #endregion

            #region Methods

            private static string BuildResponse(ResponseCode responseCode, string checksum)
            {
                var code = ((int)responseCode).ToString().PadLeft(2, '0');
                return $"{ControlCharacters.SOH}{code}{ControlCharacters.ETX}{checksum}{ControlCharacters.EOT}";
            }

            #endregion
        }

        internal static class Outgoing
        {
            #region Properties

            public static string BadSignOnMessage(IHoneywellDeviceType type)
                            => $"{ControlCharacters.SOH}SN,33333{ControlCharacters.STX}vq{type.AccessCode.ToString().PadLeft(2, '0')}{ControlCharacters.ETX}0000{ControlCharacters.EOT}";

            public static string SignOnMessage(IHoneywellDeviceType type)
                            => $"{ControlCharacters.SOH}SN,33333{ControlCharacters.STX}vq{type.AccessCode.ToString().PadLeft(2, '0')}{ControlCharacters.ETX}415D{ControlCharacters.EOT}";

            #endregion
        }

        #endregion
    }
}