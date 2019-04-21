using System.Collections.Generic;

namespace Devices.Communications.IO
{
    public static class ControlCharacters
    {
        #region Public Fields

        public const char ACK = (char)6;
        public const char Comma = (char)44;
        public const char CR = (char)13;
        public const char ENQ = (char)5;
        public const char EOT = (char)4;
        public const char ETX = (char)3;
        public const char NAK = (char)21;
        public const char RS = (char)30;
        public const char SOH = (char)1;
        public const char STX = (char)2;

        #endregion Public Fields

        #region Public Properties

        public static List<char> All => new List<char>
        {
            SOH,
            STX,
            ETX,
            EOT,
            ENQ,
            ACK,
            CR,
            NAK,
            RS
        };

        #endregion Public Properties

        #region Public Methods

        public static string Prettify(string msg)
        {
            msg = msg.Replace(SOH.FormatChar(), "[SOH]");
            msg = msg.Replace(STX.FormatChar(), "[STX]");
            msg = msg.Replace(ETX.FormatChar(), "[ETX]");
            msg = msg.Replace(EOT.FormatChar(), "[EOT]");
            msg = msg.Replace(ENQ.FormatChar(), "[ENQ]");
            msg = msg.Replace(ACK.FormatChar(), "[ACK]");
            msg = msg.Replace(CR.FormatChar(), "[CR]");
            msg = msg.Replace(NAK.FormatChar(), "[NAK]");
            msg = msg.Replace(RS.FormatChar(), "[RS]");
            return msg;
        }

        #endregion Public Methods

        #region Private Methods

        private static string FormatChar(this char c) => new string(new[] { c });

        #endregion Private Methods
    }
}