using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.Extensions
{
    public static class ControlCharacters
    {
        public static bool IsEndOfTransmission(this char c)
        {
            return c == (char)ControlChars.EOT;
        }

        public static bool IsStartOfHandshake(this char c)
        {
            return c == (char)ControlChars.SOH;
        }

        public static bool IsEndOfText(this char c)
        {
            return c == (char)ControlChars.ETX;
        }

        public static bool IsAcknowledgement(this char c)
        {
            return c == (char)ControlChars.ACK;
        }
    }
}
