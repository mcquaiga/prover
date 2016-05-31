using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.EVCTypes;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.ViewModels.InstrumentViews;
using Prover.GUI.ViewModels.VerificationTestViews;
using Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews;

namespace Prover.GUI.ViewModels.InstrumentReport
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
