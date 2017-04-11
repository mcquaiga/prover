using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prover.Domain.Instrument
{
    public interface IInstrumentFactory
    {
        string TypeString { get; }
        string Description { get; }
        Task<IInstrument> Create();
        Task<IInstrument> Create(Dictionary<string, string> itemData);
    }

    public abstract class InstrumentFactoryBase : IInstrumentFactory
    {
        protected InstrumentFactoryBase() { }
        public virtual string TypeString => this.GetType().AssemblyQualifiedName;
        public abstract string Description { get; }
        public abstract Task<IInstrument> Create();
        public abstract Task<IInstrument> Create(Dictionary<string, string> itemData);
    }
}