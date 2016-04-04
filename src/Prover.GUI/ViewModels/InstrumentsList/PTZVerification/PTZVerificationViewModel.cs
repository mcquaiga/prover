using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels.InstrumentsList.PTZVerification
{
    public class PTZVerificationViewModel
    {
        public PTZVerificationViewModel(Instrument instrument)
        {
            this.Instrument = instrument;

            instrument.VerificationTests.ForEach(v => VerificationTests.Add(new PTZVerificationSetViewModel(v)));
        }

        public Instrument Instrument { get; private set; }
        public ObservableCollection<PTZVerificationSetViewModel> VerificationTests { get; private set; } = new ObservableCollection<PTZVerificationSetViewModel>();
    }
}
