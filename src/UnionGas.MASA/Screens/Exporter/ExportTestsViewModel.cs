namespace UnionGas.MASA.Screens.Exporter
{
    using Caliburn.Micro;
    using Prover.Core.Events;
    using Prover.Core.ExternalIntegrations;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Services;
    using Prover.Core.Storage;
    using Prover.GUI.Screens;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ExportTestsViewModel" />
    /// </summary>
    public class ExportTestsViewModel : ViewModelBase, IHandle<DataStorageChangeEvent>
    {
        #region Fields

        /// <summary>
        /// Defines the _exportTestRun
        /// </summary>
        private readonly IExportTestRun _exportTestRun;

        /// <summary>
        /// Defines the _instrumentStore
        /// </summary>
        private readonly TestRunService _testRunService;

        /// <summary>
        /// Defines the _showLoadingIndicator
        /// </summary>
        private readonly ObservableAsPropertyHelper<bool> _showLoadingIndicator;

        /// <summary>
        /// Defines the _showTestViewListBox
        /// </summary>
        private readonly ObservableAsPropertyHelper<bool> _showTestViewListBox;

        /// <summary>
        /// Defines the _exportFailedTestCommand
        /// </summary>
        private ReactiveCommand _exportFailedTestCommand;

        /// <summary>
        /// Defines the _failedCompanyNumber
        /// </summary>
        private string _failedCompanyNumber;

        /// <summary>
        /// Defines the _rootResults
        /// </summary>
        private ReactiveList<QaTestRunGridViewModel> _rootResults = new ReactiveList<QaTestRunGridViewModel>() { ChangeTrackingEnabled = true };

        /// <summary>
        /// Defines the _visibleTiles
        /// </summary>
        private IReactiveDerivedList<QaTestRunGridViewModel> _visibleTiles;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportTestsViewModel"/> class.
        /// </summary>
        /// <param name="screenManager">The screenManager<see cref="ScreenManager"/></param>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="exportTestRun">The exportTestRun<see cref="IExportTestRun"/></param>
        /// <param name="testRunService">The instrumentStore<see cref="IInstrumentStore{Instrument}"/></param>
        public ExportTestsViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IExportTestRun exportTestRun, TestRunService testRunService) : base(screenManager, eventAggregator)
        {
            _exportTestRun = exportTestRun;
            _testRunService = testRunService;

            FilterObservable = new Subject<Predicate<Instrument>>();

            FilterByTypeCommand = ReactiveCommand.Create<string>(s =>
            {
                FilterObservable.OnNext(Instrument.IsOfInstrumentType(s));
            });

            ExecuteTestSearch = ReactiveCommand.CreateFromTask(LoadTests, outputScheduler: RxApp.TaskpoolScheduler);

            ExecuteTestSearch.IsExecuting
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.ShowLoadingIndicator, out _showLoadingIndicator, true);

            ExecuteTestSearch.IsExecuting
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(e => !e)
                .ToProperty(this, x => x.ShowTestViewListBox, out _showTestViewListBox, false);

            CreateTestsViewsCommand = ReactiveCommand.CreateFromTask<IEnumerable<Instrument>>(CreateInstrumentViews);

            ExecuteTestSearch
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(CreateTestsViewsCommand);
                //.Subscribe(async i => await CreateInstrumentViews(i));

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
            ExportFailedTestCommand = ReactiveCommand.CreateFromTask(ExportFailedTest);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ExecuteTestSearch
        /// </summary>
        public ReactiveCommand<Unit, IEnumerable<Instrument>> ExecuteTestSearch { get; protected set; }

        /// <summary>
        /// Gets or sets the ExportAllPassedQaRunsCommand
        /// </summary>
        public ReactiveCommand ExportAllPassedQaRunsCommand { get; set; }

        /// <summary>
        /// Gets or sets the ExportFailedTestCommand
        /// </summary>
        public ReactiveCommand ExportFailedTestCommand { get => _exportFailedTestCommand; set => this.RaiseAndSetIfChanged(ref _exportFailedTestCommand, value); }

        /// <summary>
        /// Gets or sets the FailedCompanyNumber
        /// </summary>
        public string FailedCompanyNumber { get => _failedCompanyNumber; set => this.RaiseAndSetIfChanged(ref _failedCompanyNumber, value); }

        /// <summary>
        /// Gets the FilterByTypeCommand
        /// </summary>
        public ReactiveCommand<string, Unit> FilterByTypeCommand { get; }

        /// <summary>
        /// Gets or sets the FilterObservable
        /// </summary>
        public Subject<Predicate<Instrument>> FilterObservable { get; set; }

        /// <summary>
        /// Gets or sets the PassedTests
        /// </summary>
        public IReactiveDerivedList<Instrument> PassedTests { get; set; }

        /// <summary>
        /// Gets or sets the RootResults
        /// </summary>
        public ReactiveList<QaTestRunGridViewModel> RootResults { get => _rootResults; set => this.RaiseAndSetIfChanged(ref _rootResults, value); }

        /// <summary>
        /// Gets a value indicating whether ShowLoadingIndicator
        /// </summary>
        public bool ShowLoadingIndicator => _showLoadingIndicator.Value;

        /// <summary>
        /// Gets a value indicating whether ShowTestViewListBox
        /// </summary>
        public bool ShowTestViewListBox => _showTestViewListBox.Value;

        /// <summary>
        /// Gets or sets the VisibleTiles
        /// </summary>
        public IReactiveDerivedList<QaTestRunGridViewModel> VisibleTiles { get => _visibleTiles; set => this.RaiseAndSetIfChanged(ref _visibleTiles, value); }
        public ReactiveCommand<IEnumerable<Instrument>, Unit> CreateTestsViewsCommand { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The ExportAllPassedQaRuns
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task ExportAllPassedQaRuns()
        {
            await _exportTestRun.Export(PassedTests);
        }

        /// <summary>
        /// The Handle
        /// </summary>
        /// <param name="message">The message<see cref="DataStorageChangeEvent"/></param>
        public void Handle(DataStorageChangeEvent message)
        {
        }

        /// <summary>
        /// The CreateInstrumentViews
        /// </summary>
        /// <param name="instruments">The instruments<see cref="IEnumerable{Instrument}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task CreateInstrumentViews(IEnumerable<Instrument> instruments)
        {
            foreach (var i in instruments)
            {
                await Task.Run(() =>
                {
                    var item = ScreenManager.ResolveViewModel<QaTestRunGridViewModel>();
                    item.Instrument = i;
                    item.SetFilter(FilterObservable);
                    RootResults.Add(item);
                });
            }
        }

        /// <summary>
        /// The ExportFailedTest
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task ExportFailedTest()
        {
            await _exportTestRun.ExportFailedTest(FailedCompanyNumber);
            FailedCompanyNumber = null;
        }

        /// <summary>
        /// The LoadTests
        /// </summary>
        /// <returns>The <see cref="IObservable{Instrument}"/></returns>
        private async Task<IEnumerable<Instrument>> LoadTests()
        {
            return await Task.Run(() =>
            {
                return _testRunService
                    .GetAllUnexported()
                    .OrderBy(i => i.TestDateTime);
            });
        }

        #endregion
    }
}
