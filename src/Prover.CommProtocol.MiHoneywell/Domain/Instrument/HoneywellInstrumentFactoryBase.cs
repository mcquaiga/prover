using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Models.Instrument;

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
            if (commPort == null) throw new ArgumentNullException(nameof(commPort));
            CommPort = commPort;
        }

        public async Task<IInstrument> Create()
        {
            if (CommPort == null) return await Create(new Dictionary<string, string>());

            var instrument = CreateType();
            await instrument.GetAllItems();
            instrument.InstrumentFactory = this;
            
            return instrument;
        }

        public async Task<IInstrument> Create(Dictionary<string, string> itemData)
        {
            var instrument = CreateType(itemData);
            instrument.InstrumentFactory = this;
            return instrument;
        }

        internal abstract HoneywellInstrument CreateType();
        internal abstract HoneywellInstrument CreateType(Dictionary<string, string> itemData);
        public abstract string Description { get; }
        public string TypeIdentifier => GetType().AssemblyQualifiedName;
    }
}