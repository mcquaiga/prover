using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Reports;
using Prover.GUI.Screens.QAProver.VerificationTestViews;
using Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews;
using Prover.Core.ExternalIntegrations;

namespace Prover.GUI.Screens.Export
{
    public class InstrumentTestGridViewModel : InstrumentTestViewModel
    {
        private IExportTestRun _exportManager;

        public InstrumentTestGridViewModel(IUnityContainer container, Instrument instrument)
            : base(container, instrument)
        {
            _exportManager = container.Resolve<IExportTestRun>();
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

        public async Task ExportQATestRun()
        {
            await _exportManager.Export(Instrument);
        }
    }
}