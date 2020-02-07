using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Shared.DTO.Instrument
{
    public class InstrumentDto
    {
        public string InstrumentFactory { get; set; }
        public Dictionary<string, string> ItemData { get; set; }
    }
}
