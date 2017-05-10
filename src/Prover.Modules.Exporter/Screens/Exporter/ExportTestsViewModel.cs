using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace Prover.Modules.Exporter.Screens.Exporter
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

            ExecuteTestSearch = ReactiveCommand.CreateFromTask(GetInstrumentsWithNoExportDate);
            _searchResults = ExecuteTestSearch.ToProperty(this, x => x.InstrumentItems, new List<QaTestRunGridViewModel>());

            //this.WhenAnyValue(x => x.InstrumentItems, (vm, list) =>
            //    {
            //        return .InstrumentItems.Select(x => x.Select(y => y.Instrument)
            //                .Where(i => i.HasPassed && !string.IsNullOrEmpty(i.JobId) &&
            //                            !string.IsNullOrEmpty(i.EmployeeId))
            //                .ToList());
            //    });
                            
               // .ToProperty(this, x => x.PassedInstrumentTests, new List<Instrument>());               
               
           // var canExportAllPassed = this.WhenAnyValue(x => x.PassedInstrumentTests, (passed) => passed.Any());

            ExportAllPassedQaRunsCommand = ReactiveCommand.CreateFromTask(ExportAllPassedQaRuns);
        }

        private ObservableAsPropertyHelper<List<QaTestRunGridViewModel>> _searchResults;
        public List<QaTestRunGridViewModel> InstrumentItems => _searchResults.Value;
        public ReactiveCommand<Unit, List<QaTestRunGridViewModel>> ExecuteTestSearch { get; protected set; }

        
        public IEnumerable<Instrument> PassedInstrumentTests =>
            InstrumentItems.Where(x => x.Instrument.HasPassed && !string.IsNullOrEmpty(x.Instrument.JobId) && !string.IsNullOrEmpty(x.Instrument.EmployeeId))
                .Select(i => i.Instrument);


        public ReactiveCommand ExportAllPassedQaRunsCommand { get; set; }
        public async Task ExportAllPassedQaRuns()
        {
            await _exportTestRun.Export(PassedInstrumentTests);
        }

        private async Task<List<QaTestRunGridViewModel>> GetInstrumentsWithNoExportDate()
        {
            return await Task.Run(() =>
            {
                var results = new List<QaTestRunGridViewModel>();
                var instruments = GetInstruments(x => x.CertificateId == null && x.ArchivedDateTime == null).ToList();

                foreach (var i in instruments)
                {
                    var item = ScreenManager.ResolveViewModel<QaTestRunGridViewModel>();
                    item.Instrument = i;
                    results.Add(item);
                }
                return results;
            });
        }


        private IEnumerable<Instrument> GetInstruments(Func<Instrument, bool> whereFunc)
        {
            return _instrumentStore.Query().Where(whereFunc).OrderBy(i => i.TestDateTime);
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