using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Reports;
using Prover.GUI.Screens.QAProver.VerificationTestViews;
using Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews;

namespace Prover.GUI.Screens.Export
{
    public class InstrumentTestGridViewModel : InstrumentTestViewModel
    {
        public InstrumentTestGridViewModel(IUnityContainer container, Instrument instrument)
            : base(container, instrument)
        {
            SiteInformationItem = new InstrumentInfoViewModel(container, instrument);
            VolumeInformationItem = new VolumeTestViewModel(container, instrument.VolumeTest);
        }

        public bool IsSelected { get; set; }

        public void DisplayInstrumentReport()
        {
            var instrumentReport = new InstrumentGenerator(Instrument, _container);
            instrumentReport.Generate();
        }

        public async Task DeleteInstrument()
        {
            using (var store = new InstrumentStore(_container))
            {
                await store.Delete(Instrument);
            }
        }
    }
}