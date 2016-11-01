using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Reports;

namespace UnionGas.MASA.Screens.Exporter
{
    public class QaTestRunGridViewModel : ViewModelBase
    {
        private readonly IExportTestRun _exportManager;
        private readonly InstrumentReportGenerator _instrumentReportGenerator;
        private readonly IInstrumentStore<Instrument> _instrumentStore;

        public QaTestRunGridViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IExportTestRun exportManager, IInstrumentStore<Instrument> instrumentStore,
            InstrumentReportGenerator instrumentReportGenerator) : base(screenManager, eventAggregator)
        {
            _exportManager = exportManager;
            _instrumentStore = instrumentStore;
            _instrumentReportGenerator = instrumentReportGenerator;
        }

        public Instrument Instrument { get; set; }

        public bool IsSelected { get; set; }

        public async Task DisplayInstrumentReport()
        {
            await _instrumentReportGenerator.GenerateAndViewReport(Instrument);
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