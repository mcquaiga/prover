using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using Prover.GUI.Screens;
using Prover.GUI.Screens.Modules.QAProver.Screens;

namespace Prover.GUI.Reports
{
    public class InstrumentReportViewModel : ViewModelBase
    {
        public string ReportViewContext => "EditTestViewNew";

        public InstrumentReportViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            QaTestRunViewItem = screenManager.ResolveViewModel<TestRunViewModel>();
        }

        public TestRunViewModel QaTestRunViewItem { get; set; }

        public async Task Initialize(Instrument instrument)
        {
            QaTestRunViewItem.InitializeViews(null, instrument);
            QaTestRunViewItem.ViewContext = ReportViewContext;
        }
    }
}