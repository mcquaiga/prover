using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.ViewModels.InstrumentsList
{
    public class InstrumentViewModel : ReactiveScreen
    {       
        public Instrument Instrument { get; set; }

        public int? RowNumber { get; set; }

        public InstrumentViewModel(Instrument instrument)
        {
            Instrument = instrument;
        }

        public InstrumentViewModel(Instrument instrument, int rowNumber)
            : this(instrument)
        {
            RowNumber = rowNumber;
        }

        public InstrumentTempViewModel TemperatureItem
        {
            get { return new InstrumentTempViewModel(Instrument.Temperature); }
        }

        public string HasPassed
        {
            get { return Instrument.HasPassed ? "PASS" : "FAIL"; }
        }

        public bool IsSelected { get; set; }
    }
}
