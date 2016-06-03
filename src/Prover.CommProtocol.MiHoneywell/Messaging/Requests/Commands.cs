using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Messaging;
using Prover.CommProtocol.MiHoneywell.Messaging.Response;

namespace Prover.CommProtocol.MiHoneywell.Messaging.Requests
{
    internal static class Commands
    {
        public const string DefaultAccessCode = "33333";

        public static MiCommandDefinition<StatusResponseMessage>
            WakeupOne() => new MiCommandDefinition<StatusResponseMessage>(ControlCharacters.EOT);

        public static MiCommandDefinition<StatusResponseMessage>
            WakeupTwo()
            => new MiCommandDefinition<StatusResponseMessage>(ControlCharacters.ENQ, ResponseProcessors.ResponseCode);

        public static MiCommandDefinition<StatusResponseMessage>
            SignOn(InstrumentType instrument, string accessCode = DefaultAccessCode)
        {
            var code = GetInstrumentCode(instrument);
            var cmd = $"SN,{accessCode}{ControlCharacters.STX}vq{code}";
            return new MiCommandDefinition<StatusResponseMessage>(cmd, ResponseProcessors.ResponseCode);
        }

        public static MiCommandDefinition<StatusResponseMessage> SignOffCommand()
            => new MiCommandDefinition<StatusResponseMessage>("SF", ResponseProcessors.ResponseCode);

        public static MiCommandDefinition<ItemValueResponseMessage>
            ReadItem(int itemNumber) => new ReadItemCommand(itemNumber);

        public static MiCommandDefinition<ItemGroupResponseMessage>
            ReadGroup(IEnumerable<int> itemNumbers) => new ReadGroupCommand(itemNumbers);

        private static string GetInstrumentCode(InstrumentType instrument)
           => (int)instrument < 10 ? string.Concat("0", (int)instrument) : instrument.ToString();
    }

    internal class MiCommandDefinition<TResponse> : CommandDefinition<TResponse>
        where TResponse : ResponseMessage
    {
        protected MiCommandDefinition()
        {
        }

        public MiCommandDefinition(char controlChar, ResponseProcessor<TResponse> processor = null)
        {
            Command = new string(new[] {controlChar});
            ResponseProcessor = processor;
        }

        public MiCommandDefinition(string body, ResponseProcessor<TResponse> processor)
        {
            Command = BuildCommand(body);
            ResponseProcessor = processor;
        }

        public override ResponseProcessor<TResponse> ResponseProcessor { get; }

        protected string BuildCommand(string body)
        {
            body = string.Concat(body, ControlCharacters.ETX);

            var crc = CRC.CRC.CalcCRC(body);
            return string.Concat(ControlCharacters.SOH, body, crc, ControlCharacters.EOT);
        }
    }

    internal class ReadItemCommand : MiCommandDefinition<ItemValueResponseMessage>
    {
        private const string CommandPrefix = "RD";

        public ReadItemCommand(int itemNumber)
        {
            ItemNumber = itemNumber;
            var itemString = itemNumber.ToString().PadLeft(3, Convert.ToChar("0"));
            Command = BuildCommand($"{CommandPrefix}{ControlCharacters.STX}{itemString}");
        }

        public int ItemNumber { get; }

        public override ResponseProcessor<ItemValueResponseMessage> ResponseProcessor
            => ResponseProcessors.ItemValue(ItemNumber);
    }

    internal class ReadGroupCommand : MiCommandDefinition<ItemGroupResponseMessage>
    {
        private const string CommandPrefix = "RG";

        /// <summary>
        ///     Builds the RG command for 1 to 15 item numbers
        ///     An exception is thrown is there are more than 15 items
        /// </summary>
        /// <param name="itemNumbers">Can only be 15 items maximum</param>
        /// <returns></returns>
        public ReadGroupCommand(IEnumerable<int> itemNumbers)
        {
            ItemNumbers = itemNumbers as int[] ?? itemNumbers.ToArray();
            if (ItemNumbers.Count() > 15) throw new ArgumentOutOfRangeException(nameof(itemNumbers));

            var itemsString = JoinItemValues(ItemNumbers);
            var cmd = $"{CommandPrefix}{ControlCharacters.STX}{itemsString}";
            Command = BuildCommand(cmd);
        }

        public IEnumerable<int> ItemNumbers { get; }

        public override ResponseProcessor<ItemGroupResponseMessage> ResponseProcessor => ResponseProcessors.ItemGroup(ItemNumbers);

        private static string JoinItemValues(IEnumerable<int> items)
        {
            var itemsArray = items.Select(x => x.ToString().PadLeft(3, Convert.ToChar("0"))).ToArray();
            return string.Join(",", itemsArray);
        }
    }
}