using System;
using System.Collections.Generic;
using Prover.InstrumentProtocol.Core.IO;
using Prover.InstrumentProtocol.Honeywell.CommClients;
using Prover.InstrumentProtocol.Honeywell.Domain.Instrument;

namespace Prover.InstrumentProtocol.Honeywell.Factories
{
    public class MiniAtFactory : HoneywellInstrumentFactoryBase
    {
        public int AccessCode = 33333;

        public MiniAtFactory(Func<ICommPort> commPort) : base(commPort)
        {
        }

        public override string Description => Name;
        public int Id => 3;
        public string ItemFilePath => "MiniATItems.xml";
        public string Name => "Mini-AT";

        internal override HoneywellInstrument CreateType()
        {
            var commClient = new HoneywellClient(CommPort);
            return new HoneywellInstrument(commClient, Id, AccessCode, Name, ItemFilePath);
        }

        internal override HoneywellInstrument CreateType(Dictionary<string, string> itemData)
        {
            return new HoneywellInstrument(Id, AccessCode, Name, ItemFilePath, itemData);
        }
    }
}