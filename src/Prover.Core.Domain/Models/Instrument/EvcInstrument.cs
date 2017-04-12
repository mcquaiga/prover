using System.Collections.Generic;

namespace Prover.Domain.Models.Instrument
{
    public class EvcInstrument
    {
        public string InstrumentType { get; set; }
        public Dictionary<string, string> ItemData { get; set; }
    }
}
