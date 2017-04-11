using System.Collections.Generic;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell.CommClients;

namespace Prover.CommProtocol.MiHoneywell.Domain.Instrument.MiniMax
{
    public class MiniMaxFactory : HoneywellInstrumentFactoryBase
    {
        public int AccessCode = 33333;


        public MiniMaxFactory(CommPort commPort) : base(commPort)
        {
        }

        public MiniMaxFactory()
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