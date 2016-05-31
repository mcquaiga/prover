using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Extensions;

namespace Prover.CommProtocol.MiHoneywell.Messaging.Requests
{
    internal static class Commands
    {
        private const string DefaultAccessCode = "33333";

        public static string SignOnCommand(InstrumentType instrument, string accessCode = null)
        {
            if (accessCode == null) accessCode = DefaultAccessCode;

            var code = GetInstrumentCode(instrument);
            var cmd = $"SN,{accessCode}{ProtocolChars.STX}vq{code}";
            return BuildCommand(cmd);
        }

        public static string WakeupOne() => new string(new[] { ProtocolChars.EOT });

        public static string WakeupTwo() => new string(new[] { ProtocolChars.ENQ });

        public static string SignOffCommand() => BuildCommand("SF");

        public static string ReadItemCommand(int itemNumber)
        {
            var itemString = itemNumber.ToString().PadLeft(3, Convert.ToChar("0"));
            return BuildCommand($"RD{ProtocolChars.STX}{itemString}");
        }

        /// <summary>
        /// Builds the RG command for 1 to 15 item numbers
        /// An exception is thrown is there are more than 15 items
        /// </summary>
        /// <param name="itemNumbers">Can only be 15 items maximum</param>
        /// <returns></returns>
        public static string ReadGroupCommand(IEnumerable<int> itemNumbers)
        {
            var enumerable = itemNumbers as int[] ?? itemNumbers.ToArray();
            if (enumerable.Count() > 15) throw new ArgumentOutOfRangeException(nameof(itemNumbers));

            var itemsArray = enumerable.Select(x => x.ToString().PadLeft(3, Convert.ToChar("0"))).ToArray();
            var cmd = $"RG{ProtocolChars.STX}{string.Join(",", itemsArray)}";
            return BuildCommand(cmd);
        }

        private static string GetInstrumentCode(InstrumentType instrument)
        {
            return (int) instrument < 10 ? string.Concat("0", (int) instrument) : instrument.ToString();
        }

        private static string BuildCommand(string body)
        {
            body = string.Concat(body, ProtocolChars.ETX);

            var crc = CRC.CRC.CalcCRC(body);
            return string.Concat(ProtocolChars.SOH, body, crc, ProtocolChars.EOT);
        }

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

            //public static string SOH = FormatChar(1);
            //public static string STX = FormatChar(2);
            //public static string ETX = FormatChar(3);
            //public static string EOT = FormatChar(4);
            //public static string ENQ = FormatChar(5);
            //public static string ACK = FormatChar(6);
            //public static string CR = FormatChar(13);
            //public static string NAK = FormatChar(21);
            //public static string RS = FormatChar(30);

            //private static string FormatChar(int charValue)
            //{
            //    return new string(new[] {(char) charValue});
            //}
        }
    }
}