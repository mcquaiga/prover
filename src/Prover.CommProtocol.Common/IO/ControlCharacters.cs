using System;
using System.Collections.Generic;

namespace Prover.CommProtocol.Common.IO
{
    public static class ControlCharacters
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

        public static List<char> All => new List<char>
        {
            SOH, STX, ETX, EOT, ENQ, ACK, CR, NAK, RS
        };

        public static string Prettify(string msg)
        {
            msg = msg.Replace(SOH.FormatChar(), "[SOH]");
            msg = msg.Replace(STX.FormatChar(), "[STX]");
            msg = msg.Replace(ETX.FormatChar(), "[ETX]");
            msg = msg.Replace(EOT.FormatChar(), "[EOT]");
            msg = msg.Replace(ENQ.FormatChar(), "[ENQ]");
            msg = msg.Replace(ACK.FormatChar(), "[ACK]");
            msg = msg.Replace(CR.FormatChar(),  "[CR]");
            msg = msg.Replace(NAK.FormatChar(), "[NAK]");
            msg = msg.Replace(RS.FormatChar(),  "[RS]");
            return msg;
        }

        private static string FormatChar(this char c)
        {
            return new string (new[] {c});
        }
    }

    public static class ControlCharacterExtensions
    {
        public static bool IsEndOfTransmission(this char c)
        {
            return c == (char)ControlCharacters.EOT;
        }

        public static bool IsStartOfHandshake(this char c)
        {
            return c == (char)ControlCharacters.SOH;
        }

        public static bool IsEndOfText(this char c)
        {
            return c == (char)ControlCharacters.ETX;
        }

        public static bool IsAcknowledgement(this char c)
        {
            return c == (char)ControlCharacters.ACK;
        }
    }
}
