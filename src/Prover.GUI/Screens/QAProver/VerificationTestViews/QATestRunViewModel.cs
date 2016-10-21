using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.DriveTypes;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;
using Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    public class QaTestRunViewModel : InstrumentTestViewModel
    {
        public QaTestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, Instrument instrument) : base(screenManager, eventAggregator, instrument)
        {
            SiteInformationItem = new InstrumentInfoViewModel(screenManager, eventAggregator, instrument);

            if (Instrument.VolumeTest?.DriveType is RotaryDrive)
                MeterDisplacementItem = new RotaryMeterTestViewModel((RotaryDrive)Instrument.VolumeTest.DriveType);
        }

        public RotaryMeterTestViewModel MeterDisplacementItem { get; private set; }
    }
}