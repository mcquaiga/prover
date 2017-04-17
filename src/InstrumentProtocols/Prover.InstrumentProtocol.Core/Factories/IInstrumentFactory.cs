using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.InstrumentProtocol.Core.IO;
using Prover.InstrumentProtocol.Core.Models.Instrument;

namespace Prover.InstrumentProtocol.Core.Factories
{
    public interface IInstrumentFactory
    {
        string Description { get; }
        string TypeIdentifier { get; }
        IInstrument Create();
    }
}