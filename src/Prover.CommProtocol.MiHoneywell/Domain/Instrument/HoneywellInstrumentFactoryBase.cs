using System.Threading.Tasks;
using Prover.CommProtocol.Common.IO;
using Prover.Domain.Instrument;

namespace Prover.CommProtocol.MiHoneywell.Domain.Instrument
{
    public abstract class HoneywellInstrumentFactoryBase : IInstrumentFactory
    {
        protected CommPort CommPort;

        protected HoneywellInstrumentFactoryBase()
        {
            CommPort = null;
        }

        protected HoneywellInstrumentFactoryBase(CommPort commPort)
        {
            CommPort = commPort;
        }

        public abstract string Description { get; }

        public async Task<IInstrument> Create(bool isReadOnly = false)
        {
            if (CommPort == null) isReadOnly = true;

            var instrument = CreateType(isReadOnly);
            await instrument.GetAllItems();
            return instrument;
        }

        internal abstract HoneywellInstrument CreateType(bool isReadOnly);
    }
}