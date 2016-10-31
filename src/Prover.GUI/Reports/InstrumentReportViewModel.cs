using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver.VerificationTestViews;

namespace Prover.GUI.Reports
{
    public class InstrumentReportViewModel : ViewModelBase
    {
        public InstrumentReportViewModel(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
            QaTestRunViewItem = screenManager.ResolveViewModel<QaTestRunViewModel>();
        }

        public async Task Initialize(Instrument instrument)
        {
            await QaTestRunViewItem.Initialize(instrument);
        }

        public QaTestRunViewModel QaTestRunViewItem { get; set; }
    }
}