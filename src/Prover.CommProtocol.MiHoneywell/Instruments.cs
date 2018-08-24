using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell.CommClients;

namespace Prover.CommProtocol.MiHoneywell
{
    public static class Instruments
    {
        public static InstrumentType MiniAt = new InstrumentType
        {
            Id = 3,
            AccessCode = 3,
            Name = "Mini-AT",
            ItemFilePath = "MiniATItems.xml",
            ClientFactory = port => new HoneywellClient(port, Instruments.MiniAt)
        };

        public static InstrumentType Toc = new InstrumentType
        {
            Id = 33,
            AccessCode = 3,
            Name = "TOC",
            ItemFilePath = "TOCItems.xml",
            ClientFactory = port => new TocHoneywellClient(port, Toc)
        };       

        public static InstrumentType MiniMax = new InstrumentType
        {
            Id = 4,
            AccessCode = 4,
            Name = "Mini-Max",
            ItemFilePath = "MiniMaxItems.xml",
            ClientFactory = port => new HoneywellClient(port, Instruments.MiniMax)
        };

        public static IEnumerable<InstrumentType> GetAll()
        {
            return new List<InstrumentType>
            {
                MiniAt,
                MiniMax,
                Toc
            };
        }
    }
}