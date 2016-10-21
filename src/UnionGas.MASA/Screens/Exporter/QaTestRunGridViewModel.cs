using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;

namespace UnionGas.MASA.Screens.Exporter
{
    public class QaTestRunGridViewModel : ViewModelBase
    {
        private readonly IExportTestRun _exportManager;
        private readonly IInstrumentStore<Instrument> _instrumentStore;

        public QaTestRunGridViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IExportTestRun exportManager, IInstrumentStore<Instrument> instrumentStore) : base(screenManager, eventAggregator)
        {
            _exportManager = exportManager;
            _instrumentStore = instrumentStore;
            //SiteInformationItem = new InstrumentInfoViewModel(screenManager, eventAggregator, instrument);
            //VolumeInformationItem = new VolumeTestViewModel(screenManager, eventAggregator, instrument.VolumeTest);
        }

        public Instrument Instrument { get; set; }

        public bool IsSelected { get; set; }

        public void DisplayInstrumentReport()
        {
            //var instrumentReport = new InstrumentGenerator(Instrument, _container);
            //instrumentReport.Generate();
        }

        public async Task DeleteInstrument()
        {
            await _instrumentStore.Delete(Instrument);
        }

        public async Task ExportQATestRun()
        {
            await _exportManager.Export(Instrument);
        }
    }
}