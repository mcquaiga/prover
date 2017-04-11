using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.IO;
using Prover.Domain.Instrument;

namespace Prover.CommProtocol.MiHoneywell.Domain.Instrument
{
    public abstract class HoneywellInstrumentFactoryBase : InstrumentFactoryBase
    {
        protected CommPort CommPort;

        protected HoneywellInstrumentFactoryBase()
        {
            CommPort = null;
        }

        protected HoneywellInstrumentFactoryBase(CommPort commPort)
        {
            CommPort = commPort ?? throw new ArgumentNullException(nameof(commPort));
        }

        public override async Task<IInstrument> Create()
        {
            if (CommPort == null) return await Create(new Dictionary<string, string>());

            var instrument = CreateType();
            instrument.InstrumentFactory = this;

            await instrument.GetAllItems();
            
            return instrument;
        }

        public override async Task<IInstrument> Create(Dictionary<string, string> itemData)
        {
            var instrument = CreateType(itemData);
            instrument.InstrumentFactory = this;
            return instrument;
        }

        internal abstract HoneywellInstrument CreateType();
        internal abstract HoneywellInstrument CreateType(Dictionary<string, string> itemData);
    }
}