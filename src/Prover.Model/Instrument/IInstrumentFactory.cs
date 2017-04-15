using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prover.Domain.Instrument
{
    public interface IInstrumentFactory
    {
        string Description { get; }
        string TypeString { get; }
        Task<IInstrument> Create();
        Task<IInstrument> Create(Dictionary<string, string> itemData);
    }

    public abstract class InstrumentFactoryBase : IInstrumentFactory
    {
        public abstract string Description { get; }
        public virtual string TypeString => GetType().AssemblyQualifiedName;
        public abstract Task<IInstrument> Create();
        public abstract Task<IInstrument> Create(Dictionary<string, string> itemData);
    }
}