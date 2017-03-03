using System.Collections.Generic;

namespace Prover.CommProtocol.Common.InstrumentTypes
{
    public static class Instruments
    {
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

        public static IEnumerable<InstrumentType> GetAll()
        {
            return new List<InstrumentType>
            {
                MiniAt,
                MiniMax
            };
        }
    }
}