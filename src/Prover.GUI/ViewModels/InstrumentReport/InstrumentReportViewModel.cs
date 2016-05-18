using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.EVCTypes;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.ViewModels.InstrumentViews;
using Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews;

namespace Prover.GUI.ViewModels.InstrumentReport
{
    public class InstrumentReportViewModel : InstrumentTestViewModel
    {
        public InstrumentReportViewModel(IUnityContainer container, Instrument instrument) : base(container, instrument)
        {
            SiteInformationItem = new InstrumentInfoViewModel(container, instrument);

            if (Instrument.VolumeTest.DriveType is RotaryDrive)
                MeterDisplacementItem = new RotaryMeterTestViewModel((RotaryDrive)Instrument.VolumeTest.DriveType);
        }

        public RotaryMeterTestViewModel MeterDisplacementItem { get; private set; }
    }
}
