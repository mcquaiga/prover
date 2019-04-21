using Devices.Communications.Messaging;
using System.Collections.Generic;

namespace Devices.Honeywell.Comm.Messaging.Response
{
    internal abstract class MiResponseMessage : ResponseMessage
    {
        protected MiResponseMessage()
        {
        }

        protected MiResponseMessage(string checksum)
        {
            Checksum = checksum;
        }

        public string Checksum { get; protected set; }
    }

    internal class ItemValueResponseMessage : MiResponseMessage
    {
        public ItemValueResponseMessage(int itemNumber, string rawValue, string checksum) : base(checksum)
        {
            ItemNumber = itemNumber;
            RawValue = rawValue;
        }

        public int ItemNumber { get; set; }
        public string RawValue { get; set; }
    }

    internal class ItemGroupResponseMessage : MiResponseMessage
    {
        public ItemGroupResponseMessage(Dictionary<int, string> itemValues, string checksum) : base(checksum)
        {
            ItemValues = itemValues;
        }

        public Dictionary<int, string> ItemValues { get; set; }
    }

    internal class StatusResponseMessage : MiResponseMessage
    {
        public StatusResponseMessage(ResponseCode code, string checksum) : base(checksum)
        {
            ResponseCode = code;

            if (!IsSuccess)
                Log.Warn($"Instrument Response Error - {ResponseCode}");

            Log.Debug($"Instrument Response - {ResponseCode}");
        }

        public ResponseCode ResponseCode { get; }

        public bool IsSuccess
            => ResponseCode == ResponseCode.NoError;

        public override string ToString()
        {
            return $"Code = {ResponseCode}; Checksum = {Checksum}";
        }
    }
}