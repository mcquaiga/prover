using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Messaging;
using Prover.CommProtocol.MiHoneywell.Messaging.Response;

namespace Prover.CommProtocol.MiHoneywell.Messaging.Requests
{
    internal static class Commands
    {
        public const string DefaultAccessCode = "33333";

        /// <summary>
        ///     Creates an EOT request for the instrument
        ///     Initiates the first part of the handshake
        ///     Waking the instrument up
        /// </summary>
        /// <returns>No response is expected from the instrument</returns>
        public static MiCommandDefinition<StatusResponseMessage>
            WakeupOne() => new MiCommandDefinition<StatusResponseMessage>(ControlCharacters.EOT);

        /// <summary>
        ///     Creates an ENQ request for the instrument
        ///     Second sequence of the handshake
        /// </summary>
        /// <returns>A response code is expected in return - ACK if successful</returns>
        public static MiCommandDefinition<StatusResponseMessage>
            WakeupTwo()
            => new MiCommandDefinition<StatusResponseMessage>(ControlCharacters.ENQ, ResponseProcessors.ResponseCode);

        /// <summary>
        ///     Creates an NAK request
        ///     Used when the instrument isn't responding to the first two wake ups
        ///     A last effort to try and wake the instrument up
        /// </summary>
        /// <returns>No response is expected</returns>
        public static MiCommandDefinition<StatusResponseMessage>
            OkayToSend() => new MiCommandDefinition<StatusResponseMessage>(ControlCharacters.NAK);

        /// <summary>
        ///     Creates the Sign On command to the instrument
        /// </summary>
        /// <param name="instrument">Instrument Type</param>
        /// <param name="accessCode">Password for access to the instrument</param>
        /// <returns>
        ///     A response code is expected in return
        ///     NoError indicates we're connected
        /// </returns>
        public static MiCommandDefinition<StatusResponseMessage>
            SignOn(InstrumentType instrument, string accessCode = null)
        {
            if (string.IsNullOrEmpty(accessCode))
                accessCode = DefaultAccessCode;

            var code = instrument.AccessCode < 10 ? string.Concat("0", instrument.AccessCode) : instrument.AccessCode.ToString();
            var cmd = $"SN,{accessCode}{ControlCharacters.STX}vq{code}";
            return new MiCommandDefinition<StatusResponseMessage>(cmd, ResponseProcessors.ResponseCode);
        }

        /// <summary>
        ///     Creates a Sign Off command
        /// </summary>
        /// <returns>
        ///     Expects a response code in return
        ///     NoError indicates we're disconnected cleanly
        /// </returns>
        public static MiCommandDefinition<StatusResponseMessage>
            SignOffCommand() => new MiCommandDefinition<StatusResponseMessage>("SF", ResponseProcessors.ResponseCode);

        /// <summary>
        ///     Creates a Read Item command to get a single value
        /// </summary>
        /// <param name="itemNumber">Item number to request</param>
        /// <returns></returns>
        public static MiCommandDefinition<ItemValueResponseMessage>
            ReadItem(int itemNumber) => new ReadItemCommand(itemNumber);

        /// <summary>
        ///     Creates a Read Group command to get a group of values
        /// </summary>
        /// <param name="itemNumbers">A collection of 15 item numbers maximum</param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public static MiCommandDefinition<ItemGroupResponseMessage>
            ReadGroup(IEnumerable<int> itemNumbers) => new ReadGroupCommand(itemNumbers);

        /// <summary>
        ///     Creates a Write Item command to set a value
        /// </summary>
        /// <param name="itemNumber">Item number to write</param>
        /// <param name="value">Value to write</param>
        /// <param name="accessCode">Password for the instrument</param>
        /// <returns>A response code is expected in return</returns>
        public static MiCommandDefinition<StatusResponseMessage>
            WriteItem(int itemNumber, string value, string accessCode = DefaultAccessCode)
            => new WriteItemCommand(itemNumber, value, accessCode);

        /// <summary>
        ///     Creates a Live Read command
        /// </summary>
        /// <param name="itemNumber">Item number to live read</param>
        /// <returns></returns>
        public static MiCommandDefinition<ItemValueResponseMessage>
            LiveReadItem(int itemNumber) => new LiveReadItemCommand(itemNumber);
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
            if (!body.Contains(ControlCharacters.ETX))
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

        public override ResponseProcessor<ItemGroupResponseMessage> ResponseProcessor
            => ResponseProcessors.ItemGroup(ItemNumbers);

        private static string JoinItemValues(IEnumerable<int> items)
        {
            var itemsArray = items.Select(x => x.ToString().PadLeft(3, Convert.ToChar("0"))).ToArray();
            return string.Join(",", itemsArray);
        }
    }

    internal class WriteItemCommand : MiCommandDefinition<StatusResponseMessage>
    {
        private const string CommandPrefix = "WD";

        public WriteItemCommand(int number, string value, string accessCode)
        {
            Number = number;
            Value = value;

            var numberString = Number.ToString().PadLeft(3, Convert.ToChar("0"));
            var valueString = Value.PadLeft(8, Convert.ToChar("0"));
            Command = $"{CommandPrefix},{accessCode}{ControlCharacters.STX}{numberString},{valueString}";
            Command = BuildCommand(Command);
        }

        public int Number { get; }
        public string Value { get; }

        public override ResponseProcessor<StatusResponseMessage> ResponseProcessor => ResponseProcessors.ResponseCode;
    }

    internal class LiveReadItemCommand : MiCommandDefinition<ItemValueResponseMessage>
    {
        private const string CommandPrefix = "LR";

        public LiveReadItemCommand(int itemNumber)
        {
            ItemNumber = itemNumber;
            var itemString = itemNumber.ToString().PadLeft(3, Convert.ToChar("0"));
            Command = BuildCommand($"{CommandPrefix}{ControlCharacters.STX}{itemString}");
        }

        public int ItemNumber { get; }

        public override ResponseProcessor<ItemValueResponseMessage> ResponseProcessor
            => ResponseProcessors.ItemValue(ItemNumber);
    }
}