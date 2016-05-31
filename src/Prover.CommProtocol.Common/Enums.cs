using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common
{
    public enum ControlChars
    {
        SOH = (char)1,
        STX = (char)2,
        ETX = (char)3,
        EOT = (char)4,
        ENQ = (char)5,
        ACK = (char)6,
        CR = (char)13,
        NAK = (char)21,
        RS = (char)30
    }
}
