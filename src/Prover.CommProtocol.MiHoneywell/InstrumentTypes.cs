using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.MiHoneywell
{
    public static class Instruments
    {
        public static IEnumerable<InstrumentType> InstrumentList = new List<InstrumentType>
        {
            MiniAt,
            MiniMax
        };

        public static InstrumentType MiniAt = new InstrumentType
        {
            AccessCode = 3,
            Name = "Mini-AT"
        };

        public static InstrumentType MiniMax = new InstrumentType
        {
            AccessCode = 3,
            Name = "Mini-Max"
        };

        public class InstrumentType
        {
            public int AccessCode { get; set; }
            public string Name { get; set; }
        }
    }
}
