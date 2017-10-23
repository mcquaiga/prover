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
using System.Linq.Expressions;
using Expression = System.Linq.Expressions.Expression;

namespace UnionGas.MASA.Screens.Exporter
{
    public class QaTestRunGridViewModel : ViewModelBase
    {
        private readonly IExportTestRun _exportManager;
        private readonly InstrumentReportGenerator _instrumentReportGenerator;
        private readonly IProverStore<Instrument> _instrumentStore;

        public QaTestRunGridViewModel(ScreenManager screenManager, 
                IEventAggregator eventAggregator,
                IExportTestRun exportManager,
                InstrumentStore instrumentStore,
                InstrumentReportGenerator instrumentReportGenerator) 
            : base(screenManager, eventAggregator)
        {
            _exportManager = exportManager;
            _instrumentStore = instrumentStore;
            _instrumentReportGenerator = instrumentReportGenerator;

            var canExport = this.WhenAnyValue(x => x.Instrument.JobId, x => x.Instrument.EmployeeId,
                (jobId, employeeId) => !string.IsNullOrEmpty(jobId) && !string.IsNullOrEmpty(employeeId));
            ExportQaTestRunCommand = ReactiveCommand.CreateFromTask(ExportQaTestRun, canExport);

            ArchiveTestCommand = ReactiveCommand.CreateFromTask(ArchiveTest);

            ViewQaTestReportCommand = ReactiveCommand.CreateFromTask(DisplayInstrumentReport);
        }

        #region Properties
        private bool _isShowing;
        public bool IsShowing
        {
            get => _isShowing;
            set => this.RaiseAndSetIfChanged(ref _isShowing, value);
        }

        private Instrument _instrument;
        public Instrument Instrument
        {
            get { return _instrument; } 
            set { this.RaiseAndSetIfChanged(ref _instrument, value); }
        }

        public string DateTimePretty => $"{Instrument.TestDateTime:g}";

        public bool IsSelected { get; set; }

        private ReactiveCommand _viewQaTestReportCommand;
        public ReactiveCommand ViewQaTestReportCommand
        {
            get { return _viewQaTestReportCommand; }
            set { this.RaiseAndSetIfChanged(ref _viewQaTestReportCommand, value); }
        }
        public async Task DisplayInstrumentReport()
        {
            await _instrumentReportGenerator.GenerateAndViewReport(Instrument);
        }

        private ReactiveCommand _archiveTestCommand;
        public ReactiveCommand ArchiveTestCommand
        {
            get { return _archiveTestCommand; }
            set { this.RaiseAndSetIfChanged(ref _archiveTestCommand, value); }
        }

        private bool _isRemoved;
        public bool IsRemoved
        {
            get => _isRemoved;
            set => this.RaiseAndSetIfChanged(ref _isRemoved, value);
        }
        public ReactiveCommand ExportQaTestRunCommand { get; }      
        #endregion

        public void SetFilter(IObservable<Predicate<Instrument>> filter)
        {
            filter.Subscribe(x =>
            {
                IsShowing = x(Instrument);
            });
        }

        public async Task ArchiveTest()
        {
            await _instrumentStore.Delete(Instrument);            
            IsRemoved = true;
        }
        
        public async Task ExportQaTestRun()
        {
            if (string.IsNullOrEmpty(Instrument.JobId) || string.IsNullOrEmpty(Instrument.EmployeeId))
                return;

            IsRemoved = await _exportManager.Export(Instrument);            
        }

        
    }
}