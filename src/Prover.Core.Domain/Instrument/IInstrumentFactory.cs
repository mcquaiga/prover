using System.Threading.Tasks;

namespace Prover.Domain.Instrument
{
    public interface IInstrumentFactory
    {
        string Description { get; }
        Task<IInstrument> Create(bool isReadOnly = false);
    }
}