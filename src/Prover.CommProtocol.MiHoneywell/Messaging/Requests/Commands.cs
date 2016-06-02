using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Extensions;
using Prover.CommProtocol.Common.Messaging;

namespace Prover.CommProtocol.MiHoneywell.Messaging.Requests
{
    public class MiCommandDefinition : CommandDefinition
    {
        public MiCommandDefinition()
        {
            
        }

        public MiCommandDefinition(char controlChar)
        {
            Command = new string(new[] {controlChar});
        }

        public MiCommandDefinition(string body)
        {
            Command = BuildCommand(body);
        }

        protected string BuildCommand(string body)
        {
            body = string.Concat(body, Commands.ProtocolChars.ETX);

            var crc = CRC.CRC.CalcCRC(body);
            return string.Concat(Commands.ProtocolChars.SOH, body, crc, Commands.ProtocolChars.EOT);
        }
    }

    public class SignOnCommand : MiCommandDefinition
    {
        private const string DefaultAccessCode = "33333";

        public SignOnCommand(InstrumentType instrument, string accessCode = null)
        {
            if (accessCode == null) accessCode = DefaultAccessCode;

            var code = GetInstrumentCode(instrument);
            var cmd = $"SN,{accessCode}{Commands.ProtocolChars.STX}vq{code}";
            Command = BuildCommand(cmd);
        }

        private static string GetInstrumentCode(InstrumentType instrument)
        {
            return (int)instrument < 10 ? string.Concat("0", (int)instrument) : instrument.ToString();
        }
    }

    public class ReadItemCommand : MiCommandDefinition
    {
        public ReadItemCommand(int itemNumber)
        {
            var itemString = itemNumber.ToString().PadLeft(3, Convert.ToChar("0"));
            Command = BuildCommand($"RD{Commands.ProtocolChars.STX}{itemString}");
        }
    }

    public class ReadGroupCommand : MiCommandDefinition
    {
        /// <summary>
        /// Builds the RG command for 1 to 15 item numbers
        /// An exception is thrown is there are more than 15 items
        /// </summary>
        /// <param name="itemNumbers">Can only be 15 items maximum</param>
        /// <returns></returns>
        public ReadGroupCommand(IEnumerable<int> itemNumbers)
        {
            var enumerable = itemNumbers as int[] ?? itemNumbers.ToArray();
            if (enumerable.Count() > 15) throw new ArgumentOutOfRangeException(nameof(itemNumbers));

            var itemsArray = enumerable.Select(x => x.ToString().PadLeft(3, Convert.ToChar("0"))).ToArray();
            var cmd = $"RG{Commands.ProtocolChars.STX}{string.Join(",", itemsArray)}";
            Command = BuildCommand(cmd);
        }
    }

    internal static class Commands
    {
        public static string WakeupOne() => new string(new[] { ProtocolChars.EOT });

        public static string WakeupTwo() => new string(new[] { ProtocolChars.ENQ });

        public static SignOnCommand SignOnCommand(InstrumentType instrument, string accessCode = null) => new SignOnCommand(instrument, accessCode);

        public static MiCommandDefinition SignOffCommand() => new MiCommandDefinition("SF");

        public static ReadItemCommand ReadItemCommand(int itemNumber) => new ReadItemCommand(itemNumber);

        public static ReadGroupCommand ReadGroupCommand(IEnumerable<int> itemNumbers) => new ReadGroupCommand(itemNumbers);

        internal static class ProtocolChars
        {
            public static char SOH = (char)1;
            public static char STX = (char)2;
            public static char ETX = (char)3;
            public static char EOT = (char)4;
            public static char ENQ = (char)5;
            public static char ACK = (char)6;
            public static char CR = (char)13;
            public static char NAK = (char)21;
            public static char RS = (char)30;
        }
    }
}