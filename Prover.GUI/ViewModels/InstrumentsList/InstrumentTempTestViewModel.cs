using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.ViewModels.InstrumentsList
{
    public class InstrumentTempTestViewModel
    {
        public TemperatureTest Test { get; set; }

        public InstrumentTempTestViewModel(TemperatureTest test)
        {
            Test = test;
        }
    }
}
