using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver;

namespace Prover.GUI.Reports
{
    public class InstrumentReportViewModel : ViewModelBase
    {
        public InstrumentReportViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            QaTestRunViewItem = screenManager.ResolveViewModel<TestRunViewModel>();
        }

        public TestRunViewModel QaTestRunViewItem { get; set; }
        public string ReportViewContext => "EditTestView";

        public async Task Initialize(Instrument instrument)
        {
            await QaTestRunViewItem.InitializeViews(null, instrument);
            QaTestRunViewItem.ViewContext = ReportViewContext;
        }
    }
}