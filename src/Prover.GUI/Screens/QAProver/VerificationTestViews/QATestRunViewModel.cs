using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.DriveTypes;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    public class QaTestRunViewModel : ViewModelBase
    {
        public IQaRunTestManager QaRunTestManager { get; set; }

        public QaTestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
           
        }

        public async Task Initialize(Instrument instrument)
        {
            await Task.Run(() =>
            {
                Instrument = instrument;

                SiteInformationItem = ScreenManager.ResolveViewModel<InstrumentInfoViewModel>();
                SiteInformationItem.Instrument = instrument;

                foreach (var x in Instrument.VerificationTests.OrderBy(v => v.TestNumber))
                {
                    var item = ScreenManager.ResolveViewModel<VerificationSetViewModel>();
                    item.InitializeViews(x, QaRunTestManager);
                    item.VerificationTest = x;

                    TestViews.Add(item);
                }

                if (Instrument.VolumeTest?.DriveType is RotaryDrive)
                    MeterDisplacementItem = new RotaryMeterTestViewModel((RotaryDrive)Instrument.VolumeTest.DriveType);
            });
        }

        public RotaryMeterTestViewModel MeterDisplacementItem { get; private set; }

        public Instrument Instrument { get; private set; }

        #region Views

        public InstrumentInfoViewModel SiteInformationItem { get; set; }

        public ObservableCollection<VerificationSetViewModel> TestViews { get; set; } =
            new ObservableCollection<VerificationSetViewModel>();

        public VolumeTestViewModel VolumeInformationItem { get; set; }

        #endregion
    }
}