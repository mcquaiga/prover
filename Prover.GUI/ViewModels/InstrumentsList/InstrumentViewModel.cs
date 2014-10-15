using System.Collections;
using System.Collections.Generic;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.ViewModels.InstrumentsList
{
    public class InstrumentViewModel : ReactiveScreen
    {       
        public Instrument Instrument { get; set; }

        public InstrumentViewModel(Instrument instrument)
        {
            Instrument = instrument;
        }

        public InstrumentTempViewModel TemperatureItem
        {
            get { return new InstrumentTempViewModel(Instrument.Temperature); }
        }

        public bool IsSelected { get; set; }
    }
}
