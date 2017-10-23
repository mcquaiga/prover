using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;
using System.Linq.Expressions;

namespace UnionGas.MASA.Screens.Exporter
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

            FilterObservable = new Subject<Predicate<Instrument>>();
            
            FilterByTypeCommand = ReactiveCommand.Create<string>(s =>
            {
                FilterObservable.OnNext(Instrument.IsOfInstrumentType(s));
            });           

            ExecuteTestSearch = ReactiveCommand.CreateFromObservable(()
                => LoadTests(Instrument.CanExport()));
            ExecuteTestSearch
                .Subscribe(i =>
                {
                    var item = ScreenManager.ResolveViewModel<QaTestRunGridViewModel>();
                    item.Instrument = i;
                    item.SetFilter(FilterObservable);
                    RootResults.Add(item);
                });

            VisibleTiles = RootResults.CreateDerivedCollection(t => t,
                model => model.IsShowing,
                (x, y) => x.Instrument.TestDateTime.CompareTo(y.Instrument.TestDateTime));
            VisibleTiles.ChangeTrackingEnabled = true;

            VisibleTiles.ItemChanged
                .Where(x => x.PropertyName == "IsRemoved" && x.Sender.IsRemoved)
                .Select(x => x.Sender)
                .Subscribe(x => RootResults.Remove(x));

            PassedTests = VisibleTiles.CreateDerivedCollection(x => x.Instrument,
                x => x.Instrument.HasPassed
                     && !string.IsNullOrEmpty(x.Instrument.JobId)
                     && !string.IsNullOrEmpty(x.Instrument.EmployeeId));

            ExportAllPassedQaRunsCommand = ReactiveCommand.CreateFromTask(ExportAllPassedQaRuns);
            
        }

        public ReactiveCommand<string, Unit> FilterByTypeCommand { get; }

        #region Properties

        public Subject<Predicate<Instrument>> FilterObservable { get; set; }

        private IReactiveDerivedList<QaTestRunGridViewModel>_visibleTiles;
        public IReactiveDerivedList<QaTestRunGridViewModel> VisibleTiles
        {
            get => _visibleTiles;
            set => this.RaiseAndSetIfChanged(ref _visibleTiles, value);
        }
       
        public IReactiveDerivedList<Instrument> PassedTests { get; set; }
        
        private ReactiveList<QaTestRunGridViewModel> _rootResults = new ReactiveList<QaTestRunGridViewModel>() { ChangeTrackingEnabled = true };
        public ReactiveList<QaTestRunGridViewModel> RootResults
        {
            get => _rootResults;
            set => this.RaiseAndSetIfChanged(ref _rootResults, value);
        }

        public ReactiveCommand<Unit, Instrument> ExecuteTestSearch { get; protected set; }

        public ReactiveCommand ExportAllPassedQaRunsCommand { get; set; }

        #endregion

        public async Task ExportAllPassedQaRuns()
        {
            await _exportTestRun.Export(PassedTests);
        }

        private IObservable<Instrument> LoadTests(Predicate<Instrument> whereFunc)
        {
            return _instrumentStore.Query()
                .AsEnumerable()               
                .Where(whereFunc.Invoke)
                .OrderBy(i => i.TestDateTime)
                .ToObservable();
        }

        public void Handle(DataStorageChangeEvent message)
        {
            
        }
    }
}

//this.WhenAnyValue(x => x.InstrumentItems, (vm, list) =>
//    {
//        return .InstrumentItems.Select(x => x.Select(y => y.Instrument)
//                .Where(i => i.HasPassed && !string.IsNullOrEmpty(i.JobId) &&
//                            !string.IsNullOrEmpty(i.EmployeeId))
//                .ToList());
//    });

// .ToProperty(this, x => x.PassedInstrumentTests, new List<Instrument>());               

// var canExportAllPassed = this.WhenAnyValue(x => x.PassedInstrumentTests, (passed) => passed.Any());