using Prover.CommProtocol.MiHoneywell.CommClients;

namespace Prover.CommProtocol.MiHoneywell.Domain.Instrument.MiniMax
{
    public class MiniMaxFactory : HoneywellInstrumentFactoryBase
    {
        public int AccessCode = 33333;

        public override string Description => Name;
        public int Id => 4;
        public string ItemFilePath => "MiniMaxItems.xml";
        public string Name => "Mini-Max";

        internal override HoneywellInstrument CreateType(bool isReadOnly)
        {
            if (isReadOnly)
                return new MiniMaxReadOnlyInstrument(Id, 33333, Name, ItemFilePath);

            var commClient = new HoneywellClient(CommPort);
            return new MiniMaxInstrument(commClient, Id, AccessCode, Name, ItemFilePath);
        }
    }
}