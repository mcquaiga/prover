using System.Collections.Generic;
using Newtonsoft.Json;
using Prover.InstrumentProtocol.Core.Messaging;

namespace Prover.InstrumentProtocol.Honeywell.Messaging.Response
{
    internal abstract class MiResponseMessage : ResponseMessage
    {
      
        protected MiResponseMessage(string checksum) : base()
        {
            Checksum = checksum;
        }

        public string Checksum { get; protected set; }
    }

    internal class ItemValueResponseMessage : MiResponseMessage
    {
        public ItemValueResponseMessage( int itemNumber, string rawValue, string checksum) : base(checksum)
        {
            ItemNumber = itemNumber;
            RawValue = rawValue;
        }

        public int ItemNumber { get; set; }
        public string RawValue { get; set; }
        public override string ToString()
        {
            return $"Item #{ItemNumber}; Value = {RawValue}";
        }
    }

    internal class ItemGroupResponseMessage : MiResponseMessage
    {
        public ItemGroupResponseMessage( Dictionary<int, string> itemValues, string checksum) : base(checksum)
        {
            ItemValues = itemValues;
        }

        public Dictionary<int, string> ItemValues { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(ItemValues);
        }
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

        public bool IsSuccess
            => ResponseCode == ResponseCode.NoError;

        public ResponseCode ResponseCode { get; }

        public override string ToString()
        {
            return $"Code = {ResponseCode}; Checksum = {Checksum}";
        }
    }
}