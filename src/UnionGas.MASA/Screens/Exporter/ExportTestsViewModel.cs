using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using ReactiveUI;
using System.Reactive.Linq;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Services;
using Prover.GUI.Screens;

namespace UnionGas.MASA.Screens.Exporter
{
    public class ExportTestsViewModel : ViewModelBase
    {
        private readonly IExportTestRun _exportTestRun;
        private readonly TestRunService _testRunService;

        public ExportTestsViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            TestRunService testRunService, IExportTestRun exportTestRun = null) : base(screenManager, eventAggregator)
        {
            _exportTestRun = exportTestRun;
            _testRunService = testRunService;

            InstrumentTypes.AddRange(HoneywellInstrumentTypes.GetAll().Select(i => i.Name).ToList());
            
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
        private ReactiveList<string> _instrumentTypes = new ReactiveList<string>();
        public ReactiveList<string> InstrumentTypes
        {
            get => _instrumentTypes;
            set => this.RaiseAndSetIfChanged(ref _instrumentTypes, value);
        }

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
            return _testRunService.GetAllUnexported()
                .OrderBy(i => i.TestDateTime)
                .ToObservable();
        }
    }
}