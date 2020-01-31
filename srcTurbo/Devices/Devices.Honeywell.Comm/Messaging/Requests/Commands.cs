using Devices.Communications.IO;
using Devices.Communications.Messaging;
using Devices.Honeywell.Comm.Messaging.Responses;
using Devices.Honeywell.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Honeywell.Comm.Messaging.Requests
{
    internal static class Commands
    {
        public const string DefaultPassword = "33333";

        /// <summary>
        /// Creates a Live Read command
        /// </summary>
        /// <param name="itemNumber">Item number to live read</param>
        /// <returns></returns>
        public static MiCommandDefinition<ItemValueResponseMessage>
            LiveReadItem(int itemNumber) => new LiveReadItemCommand(itemNumber);

        /// <summary>
        /// Creates an NAK request Used when the instrument isn't responding to the first two wake
        /// ups A last effort to try and wake the instrument up
        /// </summary>
        /// <returns>No response is expected</returns>
        public static MiCommandDefinition<StatusResponseMessage>
            OkayToSend() => new MiCommandDefinition<StatusResponseMessage>(ControlCharacters.NAK);

        /// <summary>
        /// Creates a Read Group command to get a group of values
        /// </summary>
        /// <param name="itemNumbers">A collection of 15 item numbers maximum</param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public static MiCommandDefinition<ItemGroupResponseMessage>
            ReadGroup(IEnumerable<int> itemNumbers) => new ReadGroupCommand(itemNumbers);

        /// <summary>
        /// Creates a Read Item command to get a single value
        /// </summary>
        /// <param name="itemNumber">Item number to request</param>
        /// <returns></returns>
        public static MiCommandDefinition<ItemValueResponseMessage>
            ReadItem(int itemNumber) => new ReadItemCommand(itemNumber);

        /// <summary>
        /// Creates a Sign Off command
        /// </summary>
        /// <returns>Expects a response code in return NoError indicates we're disconnected cleanly</returns>
        public static MiCommandDefinition<StatusResponseMessage>
            SignOffCommand() => new MiCommandDefinition<StatusResponseMessage>("SF", ResponseProcessors.ResponseCode);

        /// <summary>
        /// Creates the Sign On command to the instrument
        /// </summary>
        /// <param name="evcType">Instrument Type</param>
        /// <param name="password">Password for access to the instrument</param>
        /// <returns>A response code is expected in return NoError indicates we're connected</returns>
        public static MiCommandDefinition<StatusResponseMessage>
            SignOn(HoneywellDeviceType evcType, string password = null)
        {
            if (string.IsNullOrEmpty(password))
                password = DefaultPassword;

            var code = evcType.AccessCode.Length < 2 ? string.Concat("0", evcType.AccessCode) : evcType.AccessCode.ToString();
            var cmd = $"SN,{password}{ControlCharacters.STX}vq{code}";
            return new MiCommandDefinition<StatusResponseMessage>(cmd, ResponseProcessors.ResponseCode);
        }

        /// <summary>
        /// Creates an EOT request for the instrument Initiates the first part of the handshake
        /// Waking the instrument up
        /// </summary>
        /// <returns>No response is expected from the instrument</returns>
        public static MiCommandDefinition<StatusResponseMessage>
            WakeupOne() => new MiCommandDefinition<StatusResponseMessage>(ControlCharacters.EOT);

        /// <summary>
        /// Creates an ENQ request for the instrument Second sequence of the handshake
        /// </summary>
        /// <returns>A response code is expected in return - ACK if successful</returns>
        public static MiCommandDefinition<StatusResponseMessage>
            WakeupTwo()
            => new MiCommandDefinition<StatusResponseMessage>(ControlCharacters.ENQ, ResponseProcessors.ResponseCode);

        /// <summary>
        /// Creates a Write Item command to set a value
        /// </summary>
        /// <param name="itemNumber">Item number to write</param>
        /// <param name="value">Value to write</param>
        /// <param name="password">Password for the instrument</param>
        /// <returns>A response code is expected in return</returns>
        public static MiCommandDefinition<StatusResponseMessage>
            WriteItem(int itemNumber, string value, string password = DefaultPassword)
            => new WriteItemCommand(itemNumber, value, password);
    }

    internal class LiveReadItemCommand : MiCommandDefinition<ItemValueResponseMessage>
    {
        public int ItemNumber { get; }

        public override ResponseProcessor<ItemValueResponseMessage> ResponseProcessor
            => ResponseProcessors.ItemValue(ItemNumber);

        public LiveReadItemCommand(int itemNumber)
        {
            ItemNumber = itemNumber;
            Command = BuildCommand(CommandPrefix, itemNumber);
        }

        private const string CommandPrefix = "LR";
    }

    internal class MiCommandDefinition<TResponse> : CommandDefinition<TResponse>
            where TResponse : ResponseMessage
    {
        public override ResponseProcessor<TResponse> ResponseProcessor { get; }

        public MiCommandDefinition(char controlChar, ResponseProcessor<TResponse> processor = null)
        {
            Command = new string(new[] { controlChar });
            ResponseProcessor = processor;
        }

        public MiCommandDefinition(string body, ResponseProcessor<TResponse> processor)
        {
            Command = BuildCommand(body);
            ResponseProcessor = processor;
        }

        protected MiCommandDefinition()
        {
        }

        protected static string JoinItemValues(IEnumerable<int> items)
        {
            var itemsArray = items.Select(x => x.ToString().PadLeft(3, Convert.ToChar("0"))).ToArray();
            return string.Join(",", itemsArray);
        }

        protected string BuildCommand(string body)
        {
            if (!body.Contains(ControlCharacters.ETX))
                body = $"{body}{ControlCharacters.ETX}";

            var crc = Checksum.CalcCRC(body);

            return $"{ControlCharacters.SOH}{body}{crc}{ControlCharacters.EOT}";
        }

        protected string BuildCommand(string commandPrefix, int itemNumber)
        {
            var itemString = itemNumber.ToString().PadLeft(3, Convert.ToChar("0"));
            return BuildCommand($"{commandPrefix}{ControlCharacters.STX}{itemString}");
        }

        protected string BuildCommand(string commandPrefix, IEnumerable<int> itemNumbers)
        {
            var itemsString = JoinItemValues(itemNumbers);
            var cmd = $"{commandPrefix}{ControlCharacters.STX}{itemsString}";
            return BuildCommand(cmd);
        }

        protected string BuildCommand(string header, string text)
        {
            var cmd = $"{header}{ControlCharacters.STX}{text}";
            return BuildCommand(cmd);
        }
    }

    internal class ReadGroupCommand : MiCommandDefinition<ItemGroupResponseMessage>
    {
        public IEnumerable<int> ItemNumbers { get; }

        public override ResponseProcessor<ItemGroupResponseMessage> ResponseProcessor
            => ResponseProcessors.ItemGroup(ItemNumbers);

        /// <summary>
        /// Builds the RG command for 1 to 15 item numbers An exception is thrown is there are more
        /// than 15 items
        /// </summary>
        /// <param name="itemNumbers">Can only be 15 items maximum</param>
        /// <returns></returns>
        public ReadGroupCommand(IEnumerable<int> itemNumbers)
        {
            ItemNumbers = itemNumbers as int[] ?? itemNumbers.ToArray();
            if (ItemNumbers.Count() > 15)
                throw new ArgumentOutOfRangeException(nameof(itemNumbers));

            Command = BuildCommand(CommandPrefix, ItemNumbers);
        }

        private const string CommandPrefix = "RG";
    }

    internal class ReadItemCommand : MiCommandDefinition<ItemValueResponseMessage>
    {
        public int ItemNumber { get; }

        public override ResponseProcessor<ItemValueResponseMessage> ResponseProcessor
            => ResponseProcessors.ItemValue(ItemNumber);

        public ReadItemCommand(int itemNumber)
        {
            ItemNumber = itemNumber;
            Command = BuildCommand(CommandPrefix, ItemNumber);
        }

        private const string CommandPrefix = "RD";
    }

    internal class WriteItemCommand : MiCommandDefinition<StatusResponseMessage>
    {
        public int Number { get; }
        public override ResponseProcessor<StatusResponseMessage> ResponseProcessor => ResponseProcessors.ResponseCode;
        public string Value { get; }

        public WriteItemCommand(int number, string value, string password = null)
        {
            Number = number;
            Value = value;
            Command = BuildWriteCommand(password);
        }

        private const string CommandPrefix = "WD";

        private string BuildWriteCommand(string password)
        {
            var numberString = Number.ToString().PadLeft(3, Convert.ToChar("0"));
            var valueString = Value.PadLeft(8, Convert.ToChar("0"));

            return BuildCommand($"{CommandPrefix},{password}", $"{numberString},{valueString}");
        }
    }
}