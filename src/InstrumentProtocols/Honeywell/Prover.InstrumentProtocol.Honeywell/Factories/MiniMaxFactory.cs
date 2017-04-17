using System;
using System.Collections.Generic;
using Prover.InstrumentProtocol.Core.IO;
using Prover.InstrumentProtocol.Honeywell.CommClients;
using Prover.InstrumentProtocol.Honeywell.Domain.Instrument;
using Prover.InstrumentProtocol.Honeywell.Domain.Instrument.MiniMax;

namespace Prover.InstrumentProtocol.Honeywell.Factories
{
    public class MiniMaxFactory : HoneywellInstrumentFactoryBase
    {
        public int AccessCode = 33333;

        public MiniMaxFactory(Func<ICommPort> commPort) : base(commPort)
        {
        }

        public override string Description => Name;
        public int Id => 4;
        public string ItemFilePath => "MiniMaxItems.xml";
        public string Name => "Mini-Max";

        internal override HoneywellInstrument CreateType()
        {
            var commClient = new HoneywellClient(CommPort);
            return new MiniMaxInstrument(commClient, Id, AccessCode, Name, ItemFilePath);
        }

        internal override HoneywellInstrument CreateType(Dictionary<string, string> itemData)
        {
            return new MiniMaxInstrument(Id, AccessCode, Name, ItemFilePath, itemData);
        }
    }
}