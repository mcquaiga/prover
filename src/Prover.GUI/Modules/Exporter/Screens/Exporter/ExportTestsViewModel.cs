using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace Prover.GUI.Modules.Exporter.Screens.Exporter
{
    public class ExportTestsViewModel : ViewModelBase, IHandle<DataStorageChangeEvent>
    {
        private readonly IExportTestRun _exportTestRun;
        private readonly IProverStore<Instrument> _instrumentStore;

        public ExportTestsViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Instrument> instrumentStore, IExportTestRun exportTestRun = null) : base(screenManager, eventAggregator)
        {
            _exportTestRun = exportTestRun;
            _instrumentStore = instrumentStore;
            GetInstrumentsWithNoExportDate();
            
            var canExportAllPassed = this.WhenAnyValue(x => x.PassedInstrumentTests, 
                (passed) => passed.Any());          
            ExportAllPassedQaRunsCommand = ReactiveCommand.CreateFromTask(ExportAllPassedQaRuns, canExportAllPassed);
        }

        public ObservableCollection<QaTestRunGridViewModel> InstrumentItems { get; set; } =
            new ObservableCollection<QaTestRunGridViewModel>();

        public IEnumerable<Instrument> PassedInstrumentTests => 
            InstrumentItems.Where(x => x.Instrument.HasPassed && !string.IsNullOrEmpty(x.Instrument.JobId) && !string.IsNullOrEmpty(x.Instrument.EmployeeId))
                .Select(i => i.Instrument);

        public ReactiveCommand ExportAllPassedQaRunsCommand { get; set; }
        public async Task ExportAllPassedQaRuns()
        {
            await _exportTestRun.Export(PassedInstrumentTests);
        }

        private void GetInstrumentsWithNoExportDate()
        {
            GetInstrumentVerificationTests(x => x.ExportedDateTime == null && x.ArchivedDateTime == null);
        }

        private void GetInstrumentVerificationTests(Func<Instrument, bool> whereFunc)
        {
            InstrumentItems.Clear();

            var instruments = _instrumentStore.Query()
                .Where(whereFunc)
                .OrderBy(i => i.TestDateTime)
                .ToList();

            foreach (var i in instruments)
            {
                var item = ScreenManager.ResolveViewModel<QaTestRunGridViewModel>();
                item.Instrument = i;
                InstrumentItems.Add(item);
            }

            NotifyOfPropertyChange(() => InstrumentItems);
        }

        public void Handle(DataStorageChangeEvent message)
        {
            GetInstrumentsWithNoExportDate();
        }
    }
}