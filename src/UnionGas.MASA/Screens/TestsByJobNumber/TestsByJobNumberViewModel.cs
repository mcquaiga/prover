namespace UnionGas.MASA.Screens.TestsByJobNumber
{
    using Caliburn.Micro;
    using Prover.Core.Services;
    using Prover.GUI.Screens;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using UnionGas.MASA.DCRWebService;
    using UnionGas.MASA.Dialogs.MeterDTODialog;

    /// <summary>
    /// Defines the <see cref="TestsByJobNumberViewModel" />
    /// </summary>
    public class TestsByJobNumberViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Defines the _jobNumbers
        /// </summary>
        internal readonly ObservableAsPropertyHelper<List<string>> _jobNumbers;

        /// <summary>
        /// Defines the _dcrWebService
        /// </summary>
        private readonly DCRWebServiceCommunicator _dcrWebService;

        /// <summary>
        /// Defines the _instrumentStore
        /// </summary>
        private readonly TestRunService _testRunService;

        /// <summary>
        /// Defines the _meterDtoList
        /// </summary>
        private IList<MeterDTO> _meterDtoList = new List<MeterDTO>();

        /// <summary>
        /// Defines the _selectedJobNumber
        /// </summary>
        private string _selectedJobNumber;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestsByJobNumberViewModel"/> class.
        /// </summary>
        /// <param name="screenManager">The screenManager<see cref="ScreenManager"/></param>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="instrumentStore">The instrumentStore<see cref="IInstrumentStore{Instrument}"/></param>
        /// <param name="dcrWebService">The dcrWebService<see cref="DCRWebServiceCommunicator"/></param>
        public TestsByJobNumberViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            TestRunService testRunService,
            DCRWebServiceCommunicator dcrWebService) : base(screenManager, eventAggregator)
        {
            _testRunService = testRunService;
            _dcrWebService = dcrWebService;

            /* Get list of open job numbers */
            GetOpenJobNumbersCommand = ReactiveCommand.Create(() =>
            {                
                return _testRunService.GetAllUnexported()
                    .Where(i => !string.IsNullOrEmpty(i.JobId))
                    .Select(i => i.JobId)
                    .Distinct()
                    .ToList();
            });

            GetOpenJobNumbersCommand
                .ToProperty(this, x => x.JobNumbers, out _jobNumbers, new List<string>());
            /* End Get open job numbers */

            /* Fetch MeterDTOs from web service */
            var canExecuteFetchTestsByJobNumber = this.WhenAnyValue(x => x.SelectedJobNumber, jobNumber => !string.IsNullOrEmpty(jobNumber));
            FetchTestsByJobNumberCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!string.IsNullOrEmpty(SelectedJobNumber))
                {
                    MeterDtoList = await _dcrWebService.GetOutstandingMeterTestsByJobNumber(Int32.Parse(SelectedJobNumber));

                    if (MeterDtoList.Any())
                    {
                        var mdViewModel = new MeterDtoListDialogViewModel(ScreenManager, EventAggregator, MeterDtoList);
                        ScreenManager.ShowDialog(mdViewModel);
                    }                    
                }                
            }, canExecuteFetchTestsByJobNumber);

            FetchTestsByJobNumberCommand.ThrownExceptions
                .Subscribe(async ex => await ScreenManager.ShowMessageBox(ex.Message));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the FetchTestsByJobNumberCommand
        /// </summary>
        public ReactiveCommand<Unit, Unit> FetchTestsByJobNumberCommand { get; private set; }

        /// <summary>
        /// Gets the GetOpenJobNumbersCommand
        /// </summary>
        public ReactiveCommand<Unit, List<string>> GetOpenJobNumbersCommand { get; }

        /// <summary>
        /// Gets the JobNumbers
        /// </summary>
        public List<string> JobNumbers => _jobNumbers.Value;

        /// <summary>
        /// Gets or sets the MeterDtoList
        /// </summary>
        public IList<MeterDTO> MeterDtoList { get => _meterDtoList; set => this.RaiseAndSetIfChanged(ref _meterDtoList, value); }

        /// <summary>
        /// Gets or sets the SelectedJobNumber
        /// </summary>
        public string SelectedJobNumber
        {
            get { return _selectedJobNumber; }
            set => this.RaiseAndSetIfChanged(ref _selectedJobNumber, value);
        }

        #endregion

        #region Methods

      

        #endregion
    }
}
