using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.GUI.ViewModels.InstrumentViews;

namespace Prover.GUI.ViewModels.InstrumentReport
{
    public class InstrumentReportViewModel : InstrumentTestViewModel
    {
        public InstrumentReportViewModel(IUnityContainer container, Instrument instrument) : base(container, instrument)
        {
            SiteInformationItem = new InstrumentInfoViewModel(container, instrument);
            VolumeInformationItem = new VolumeVerificationViewModel(container, instrument);
        }
    }
}
