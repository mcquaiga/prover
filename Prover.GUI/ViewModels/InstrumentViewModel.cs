using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.ViewModels
{
    public class InstrumentViewModel
    {
        public Instrument Instrument { get; set; }
        public InstrumentViewModel(Instrument instrument)
        {
            Instrument = instrument;
        }
    }
}
