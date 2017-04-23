#region

using System.Collections.Generic;

#endregion

namespace Prover.Core.Shared.DTO.Instrument
{
    public class InstrumentDto
    {
        public string InstrumentFactory { get; set; }
        public Dictionary<string, string> ItemData { get; set; }
    }
}