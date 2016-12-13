using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Reports;
using ReactiveUI;

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

            var canExport = this.WhenAnyValue(x => x.Instrument.JobId, x => x.Instrument.EmployeeId,
                (jobId, employeeId) => !string.IsNullOrEmpty(jobId) && !string.IsNullOrEmpty(employeeId));
            ExportQaTestRunCommand = ReactiveCommand.Create(canExport);
            ExportQaTestRunCommand.Subscribe(async _ => await ExportQaTestRun());
        }

        public Instrument Instrument { get; set; }

        public string DateTimePretty => $"{Instrument.TestDateTime:M/dd/yyyy h:mm tt}";

        public bool IsSelected { get; set; }

        public async Task DisplayInstrumentReport()
        {
            await _instrumentReportGenerator.GenerateAndViewReport(Instrument);
        }

        public async Task DeleteInstrument()
        {
            await _instrumentStore.Delete(Instrument);
            await EventAggregator.PublishOnUIThreadAsync(new DataStorageChangeEvent());
        }

        public ReactiveCommand<object> ExportQaTestRunCommand { get; }
        public async Task ExportQaTestRun()
        {
            await _exportManager.Export(Instrument);
            await EventAggregator.PublishOnUIThreadAsync(new DataStorageChangeEvent());
        }
    }
}