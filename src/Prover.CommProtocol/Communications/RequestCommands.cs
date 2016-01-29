using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Communications
{
    public static class Commands
    {
        private const int AccessCode = 33333;

        public static string WakeupOne()
        {
            return ProtocolChars.EOT;
        }

        public static string WakeupTwo()
        {
            return ProtocolChars.ENQ;
        }

        public static string SignOnCommand(InstrumentType instrument)
        {
            var template = "SN,{0}{1}vq{2}"; //
            string code = GetInstrumentCode(instrument);
            var cmd = string.Format(template, AccessCode, ProtocolChars.STX, code);
            return BuildCommand(cmd);
        }

        public static string SignOffCommand()
        {
            return BuildCommand("SF");
        }

        private static string GetInstrumentCode(InstrumentType instrument)
        {
            return (int)instrument < 10 ? string.Concat("0", (int)instrument) : instrument.ToString();
        }

        private static string BuildCommand(string body)
        {
            body = string.Concat(body, ProtocolChars.ETX);

            var crc = CRC.CRC.CalcCRC(body);
            return string.Concat(ProtocolChars.SOH, body, crc, ProtocolChars.EOT);
        }

        public static class ProtocolChars
        {
            public static string SOH = FormatChar(1);
            public static string STX = FormatChar(2);
            public static string ETX = FormatChar(3);
            public static string EOT = FormatChar(4);
            public static string ENQ = FormatChar(5);
            public static string ACK = FormatChar(6);
            public static string CR = FormatChar(13);
            public static string NAK = FormatChar(21);
            public static string RS = FormatChar(30);

            private static string FormatChar(int charValue)
            {
                return new string(new[] { (char)charValue });
            }
        }
    }
}
