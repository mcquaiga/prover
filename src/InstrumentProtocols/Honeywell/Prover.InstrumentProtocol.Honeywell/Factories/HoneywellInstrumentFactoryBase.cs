using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.InstrumentProtocol.Core.Factories;
using Prover.InstrumentProtocol.Core.IO;
using Prover.InstrumentProtocol.Core.Models.Instrument;
using Prover.InstrumentProtocol.Honeywell.Domain.Instrument;

namespace Prover.InstrumentProtocol.Honeywell.Factories
{
    public abstract class HoneywellInstrumentFactoryBase : IInstrumentFactory
    {
        protected readonly Func<ICommPort> CommPortFactory;
        public ICommPort CommPort { get; set; }

        protected HoneywellInstrumentFactoryBase(ICommPort commPort)
        {
            if (commPort == null) throw new ArgumentNullException(nameof(commPort));
            CommPort = commPort;
        }

        protected HoneywellInstrumentFactoryBase(Func<ICommPort> commPortFactory)
        {
            CommPortFactory = commPortFactory;
        }

        public abstract string Description { get; }
        public string TypeIdentifier => GetType().AssemblyQualifiedName;

        public IInstrument Create()
        {
            CommPort = CommPortFactory.Invoke();

            var instrument = CreateType();
            instrument.InstrumentFactory = this;

            return instrument;
        }

        internal abstract HoneywellInstrument CreateType();
        internal abstract HoneywellInstrument CreateType(Dictionary<string, string> itemData);
    }
}