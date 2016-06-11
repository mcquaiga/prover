using Microsoft.Practices.Unity;
using Prover.Core.DriveTypes;
using Prover.Core.Models.Instruments;
using Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    public class QaTestRunViewModel : InstrumentTestViewModel
    {
        public QaTestRunViewModel(IUnityContainer container, Instrument instrument) : base(container, instrument)
        {
            SiteInformationItem = new InstrumentInfoViewModel(container, instrument);

            if (Instrument.VolumeTest.DriveType is RotaryDrive)
                MeterDisplacementItem = new RotaryMeterTestViewModel((RotaryDrive) Instrument.VolumeTest.DriveType);
        }

        public RotaryMeterTestViewModel MeterDisplacementItem { get; private set; }
    }
}