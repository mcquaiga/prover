using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;

namespace Prover.CommProtocol.MiHoneywell
{
    public static class Instruments
    {
        public static IEnumerable<InstrumentType> GetAll()
        {
            return new List<InstrumentType>()
            {
                MiniAt,
                MiniMax
            };
        }

        public static InstrumentType MiniAt = new InstrumentType
        {
            Id = 3,
            AccessCode = 3,
            Name = "Mini-AT",
            ItemFilePath = "MiniATItems.xml"

        };

        public static InstrumentType MiniMax = new InstrumentType
        {
            Id = 4,
            AccessCode = 4,
            Name = "Mini-Max",
            ItemFilePath = "MiniMaxItems.xml"
        };
    }
}
