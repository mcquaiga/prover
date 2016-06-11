using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.GUI.Screens.QAProver.VerificationTestViews;

namespace Prover.GUI.Reports
{
    public class InstrumentReportViewModel
    {
        public InstrumentReportViewModel(IUnityContainer container, Instrument instrument)
        {
            QaTestRunViewItem = new QaTestRunViewModel(container, instrument);
        }

        public QaTestRunViewModel QaTestRunViewItem { get; set; }
    }
}