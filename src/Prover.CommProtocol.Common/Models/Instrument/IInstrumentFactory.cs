using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.Models.Instrument
{
    public interface IInstrumentFactory
    {
        string Description { get; }
        string TypeIdentifier { get; }
        Task<IInstrument> Create();
        Task<IInstrument> Create(Dictionary<string, string> itemData);
    }
}