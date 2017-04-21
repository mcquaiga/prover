using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Modules.QAProver.Screens;

namespace Prover.GUI.Reports
{
    public class InstrumentReportViewModel : ViewModelBase
    {
        public string ReportViewContext => "EditTestView";

        public InstrumentReportViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            QaTestRunViewItem = screenManager.ResolveViewModel<TestRunViewModel>();
        }

        public TestRunViewModel QaTestRunViewItem { get; set; }

        public async Task Initialize(Instrument instrument)
        {
            await QaTestRunViewItem.InitializeViews(null, instrument);
            QaTestRunViewItem.ViewContext = ReportViewContext;
        }
    }
}