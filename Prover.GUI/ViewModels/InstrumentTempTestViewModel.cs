using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.ViewModels
{
    public class InstrumentTempTestViewModel : ReactiveScreen
    {
        public TemperatureTest Test;
        public InstrumentTempTestViewModel(TemperatureTest test)
        {
            Test = test;
        }
    }
}
