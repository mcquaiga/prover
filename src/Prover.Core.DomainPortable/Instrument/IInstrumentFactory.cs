using System.Threading.Tasks;

namespace Prover.Core.DomainPortable.Instrument
{
    public interface IInstrumentFactory
    {
        string Description { get; }
        Task<IInstrument> Create(bool isReadOnly = false);
    }
}