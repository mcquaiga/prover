using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.DriveTypes;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    public class QaTestRunViewModel : ViewModelBase
    {
        public QaTestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
            SiteInformationItem = ScreenManager.ResolveViewModel<InstrumentInfoViewModel>();
            SiteInformationItem.Instrument = 

            if (Instrument.VolumeTest?.DriveType is RotaryDrive)
                MeterDisplacementItem = new RotaryMeterTestViewModel((RotaryDrive)Instrument.VolumeTest.DriveType);
        }

        public RotaryMeterTestViewModel MeterDisplacementItem { get; private set; }
    }
}