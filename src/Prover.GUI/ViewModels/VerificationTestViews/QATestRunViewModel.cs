using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prover.Core.DriveTypes;
using Prover.Core.EVCTypes;
using Prover.Core.Models.Instruments;
using Prover.GUI.ViewModels.InstrumentViews;
using Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews;

namespace Prover.GUI.ViewModels.VerificationTestViews
{
    public class QaTestRunViewModel : InstrumentTestViewModel
    {
        public QaTestRunViewModel(IUnityContainer container, Instrument instrument) : base(container, instrument)
        {
            SiteInformationItem = new InstrumentInfoViewModel(container, instrument);

            if (Instrument.VolumeTest.DriveType is RotaryDrive)
                MeterDisplacementItem = new RotaryMeterTestViewModel((RotaryDrive)Instrument.VolumeTest.DriveType);
        }

        public RotaryMeterTestViewModel MeterDisplacementItem { get; private set; }

       
    }
}
