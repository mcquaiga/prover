namespace Prover.GUI.Screens.QAProver
{
    using Caliburn.Micro;
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.Core.Events;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Settings;
    using Prover.Core.VerificationTests;
    using Prover.GUI.Common;
    using Prover.GUI.Common.Screens;
    using Prover.GUI.Screens.QAProver.PTVerificationViews;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Defines the <see cref="TestRunViewModel" />
    /// </summary>
    public class TestRunViewModel : ViewModelBase, IDisposable, IHandle<ConnectionStatusEvent>, IHandle<SaveTestEvent>
    {
        #region Constants

        /// <summary>
        /// Defines the EditQaTestViewContext
        /// </summary>
        private const string EditQaTestViewContext = "EditTestView";

        /// <summary>
        /// Defines the NewQaTestViewContext
        /// </summary>
        private const string NewQaTestViewContext = "NewTestView";

        #endregion

        #region Fields

        /// <summary>
        /// Defines the _selectedInstrument
        /// </summary>
        private readonly ObservableAsPropertyHelper<InstrumentType> _selectedInstrument;

        /// <summary>
        /// Defines the _connectionStatusMessage
        /// </summary>
        private string _connectionStatusMessage;

        /// <summary>
        /// Defines the _instruments
        /// </summary>
        private ReactiveList<SelectableInstrumentType> _instruments;
        private CommPort _commPort;

        /// <summary>
        /// Defines the _qaTestRunManager
        /// </summary>
        private ITestRunManager _qaTestRunManager;

        /// <summary>
        /// Defines the _selectedBaudRate
        /// </summary>
        private int _selectedBaudRate;

        /// <summary>
        /// Defines the _selectedCommPort
        /// </summary>
        private string _selectedCommPort;

        /// <summary>
        /// Defines the _selectedTachCommPort
        /// </summary>
        private string _selectedTachCommPort;

        /// <summary>
        /// Defines the _showConnectionDialog
        /// </summary>
        private bool _showConnectionDialog;

        /// <summary>
        /// Defines the _testStatusSubscription
        /// </summary>
        private IDisposable _testStatusSubscription;

        /// <summary>
        /// Defines the _viewContext
        /// </summary>
        private string _viewContext;
        private InstrumentInfoViewModel _siteInformationItem;
        private VolumeTestViewModel _volumeTestView;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRunViewModel"/> class.
        /// </summary>
        /// <param name="screenManager">The screenManager<see cref="ScreenManager"/></param>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        public TestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            /***  Setup Instruments list  ***/
            Instruments = new ReactiveList<SelectableInstrumentType>
            {
                ChangeTrackingEnabled = true
            };

            Instruments.ItemChanged
                .Where(x => x.PropertyName == "IsSelected" && x.Sender.IsSelected)
                .Select(x => x.Sender.Instrument)
                .ToProperty(this, x => x.SelectedInstrument, out _selectedInstrument);

            this.WhenAnyValue(x => x.SelectedInstrument)
                .Where(i => i != null)
                .Subscribe(x =>
                {
                    SettingsManager.SettingsInstance.LastInstrumentTypeUsed = x.Name;
                });

            CommProtocol.MiHoneywell.Instruments.GetAll().ToList().ForEach(
                x => Instruments.Add(new SelectableInstrumentType
                {
                    Instrument = x,
                    IsSelected = false
                }));

            var lastInstrument = Instruments.FirstOrDefault(x =>
                x.Instrument.Name == SettingsManager.SettingsInstance.LastInstrumentTypeUsed);
            if (lastInstrument != null)
                lastInstrument.IsSelected = true;

            /***  Setup Comm Ports and Baud Rate settings ***/
            _selectedCommPort = CommPort.Contains(SettingsManager.SettingsInstance.InstrumentCommPort)
                ? SettingsManager.SettingsInstance.InstrumentCommPort
                : string.Empty;

            _selectedBaudRate = BaudRate.Contains(SettingsManager.SettingsInstance.InstrumentBaudRate)
                ? SettingsManager.SettingsInstance.InstrumentBaudRate
                : -1;
            _selectedTachCommPort = TachCommPort.Contains(SettingsManager.SettingsInstance.TachCommPort)
                ? SettingsManager.SettingsInstance.TachCommPort
                : string.Empty;

            this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort)
                .Subscribe(async _ =>
                {
                    SettingsManager.SettingsInstance.InstrumentBaudRate = _.Item1;
                    SettingsManager.SettingsInstance.InstrumentCommPort = _.Item2;
                    SettingsManager.SettingsInstance.TachCommPort = _.Item3;
                });

            /*** Commands ***/
            var canStartNewTest = this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort,
                (baud, instrumentPort, tachPort) =>
                    BaudRate.Contains(baud) && !string.IsNullOrEmpty(instrumentPort) && !string.IsNullOrEmpty(tachPort));

            StartTestCommand = ReactiveCommand.CreateFromTask(StartNewQaTest, canStartNewTest);

            NextTestCommand = ReactiveCommand.CreateFromTask(StartNextTest);

            this.TestViews.ItemsAdded.Subscribe(_ =>
            {
                VolumeTestView = TestViews.FirstOrDefault(x => x.VolumeTestViewModel != null)?.VolumeTestViewModel;
            });

            _viewContext = NewQaTestViewContext;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the BaudRate
        /// </summary>
        public List<int> BaudRate => SerialPort.BaudRates;

        /// <summary>
        /// Gets the CommPort
        /// </summary>
        public List<string> CommPort => SerialPort.GetPortNames().ToList();

        /// <summary>
        /// Gets or sets the ConnectionStatusMessage
        /// </summary>
        public string ConnectionStatusMessage
        {
            get { return _connectionStatusMessage; }
            set { this.RaiseAndSetIfChanged(ref _connectionStatusMessage, value); }
        }

        /// <summary>
        /// Gets or sets the EventLogCommPortItem
        /// </summary>
        public InstrumentInfoViewModel EventLogCommPortItem { get; set; }

        /// <summary>
        /// Gets or sets the Instruments
        /// </summary>
        public ReactiveList<SelectableInstrumentType> Instruments
        {
            get { return _instruments; }
            set { this.RaiseAndSetIfChanged(ref _instruments, value); }
        }

        /// <summary>
        /// Gets the NextTestCommand
        /// </summary>
        public ReactiveCommand NextTestCommand { get; }

        /// <summary>
        /// Gets or sets the SelectedBaudRate
        /// </summary>
        public int SelectedBaudRate
        {
            get { return _selectedBaudRate; }
            set { this.RaiseAndSetIfChanged(ref _selectedBaudRate, value); }
        }

        /// <summary>
        /// Gets or sets the SelectedCommPort
        /// </summary>
        public string SelectedCommPort
        {
            get { return _selectedCommPort; }
            set { this.RaiseAndSetIfChanged(ref _selectedCommPort, value); }
        }

        /// <summary>
        /// Gets the SelectedInstrument
        /// </summary>
        public InstrumentType SelectedInstrument => _selectedInstrument.Value;

        /// <summary>
        /// Gets or sets the SelectedTachCommPort
        /// </summary>
        public string SelectedTachCommPort
        {
            get { return _selectedTachCommPort; }
            set { this.RaiseAndSetIfChanged(ref _selectedTachCommPort, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowConnectionDialog
        /// </summary>
        public bool ShowConnectionDialog
        {
            get { return _showConnectionDialog; }
            set { this.RaiseAndSetIfChanged(ref _showConnectionDialog, value); }
        }

        /// <summary>
        /// Gets a value indicating whether ShowNextTestButton
        /// </summary>
        public bool ShowNextTestButton { get; private set; } = false;

        /// <summary>
        /// Gets or sets the SiteInformationItem
        /// </summary>      
        public InstrumentInfoViewModel SiteInformationItem 
        { 
            get {return _siteInformationItem; }
            set {this.RaiseAndSetIfChanged(ref _siteInformationItem, value); }
        }

        /// <summary>
        /// Gets the StartTestCommand
        /// </summary>
        public ReactiveCommand StartTestCommand { get; }

        /// <summary>
        /// Gets the TachCommPort
        /// </summary>
        public List<string> TachCommPort => SerialPort.GetPortNames().ToList();

        /// <summary>
        /// Gets or sets the TestViews
        /// </summary>
        ///
        public ReactiveList<VerificationSetViewModel> TestViews { get; set; } =
            new ReactiveList<VerificationSetViewModel>() {                
            };

        /// <summary>
        /// Gets or sets the ViewContext
        /// </summary>
        public string ViewContext
        {
            get { return _viewContext; }
            set { this.RaiseAndSetIfChanged(ref _viewContext, value); }
        }

        /// <summary>
        /// Gets the VolumeTestView
        /// </summary>
        public VolumeTestViewModel VolumeTestView
        {
            get { return _volumeTestView;}
            set {this.RaiseAndSetIfChanged(ref _volumeTestView, value); }
        }          

        #endregion

        #region Methods

        /// <summary>
        /// The CancelCommand
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task CancelCommand()
        {
            await ScreenManager.GoHome();
        }

        /// <summary>
        /// The Dispose
        /// </summary>
        public void Dispose()
        {
            _testStatusSubscription?.Dispose();
            _qaTestRunManager?.Dispose();
        }

        /// <summary>
        /// The Handle
        /// </summary>
        /// <param name="message">The message<see cref="ConnectionStatusEvent"/></param>
        public void Handle(ConnectionStatusEvent message)
        {
            ConnectionStatusMessage = $"Attempt {message.ConnectionStatus} of {message.MaxAttempts}...";
        }

        /// <summary>
        /// The Handle
        /// </summary>
        /// <param name="message">The message<see cref="SaveTestEvent"/></param>
        public void Handle(SaveTestEvent message)
        {
            _qaTestRunManager?.SaveAsync();
        }

        /// <summary>
        /// The InitializeViews
        /// </summary>
        /// <param name="qaTestRunTestManager">The qaTestRunTestManager<see cref="ITestRunManager"/></param>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public void InitializeViews(ITestRunManager qaTestRunTestManager, Instrument instrument)
        {
            SiteInformationItem = null;
            TestViews.Clear();

            SiteInformationItem = ScreenManager.ResolveViewModel<InstrumentInfoViewModel>();
            SiteInformationItem.Initialize(instrument, qaTestRunTestManager);
            
            foreach (var x in instrument.VerificationTests.OrderBy(v => v.TestNumber))
            {               
                var item = ScreenManager.ResolveViewModel<VerificationSetViewModel>();
                item.InitializeViews(x, qaTestRunTestManager);
                item.VerificationTest = x;

                TestViews.Add(item);
            }

            if (instrument.InstrumentType == CommProtocol.MiHoneywell.Instruments.MiniAt)
            {
                EventLogCommPortItem = SiteInformationItem;
            }

            if (SelectedInstrument == CommProtocol.MiHoneywell.Instruments.Toc)
            {
                ShowNextTestButton = true;
            }
            
        }

        /// <summary>
        /// The GetCommPort
        /// </summary>
        /// <returns>The <see cref="CommPort"/></returns>
        private static CommPort GetCommPort()
        {
            return new SerialPort(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate);
        }

        /// <summary>
        /// The OnTestStatusChange
        /// </summary>
        /// <param name="status">The status<see cref="string"/></param>
        private void OnTestStatusChange(string status)
        {
            ConnectionStatusMessage = status;
        }

        /// <summary>
        /// The StartNewQaTest
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task StartNewQaTest()
        {
            ShowConnectionDialog = true;

            if (SelectedInstrument != null)
            {
                try
                {
                    await SettingsManager.Save();

                    _commPort = GetCommPort();
                    _qaTestRunManager = await TestRunCreator.CreateTestRun(SelectedInstrument, _commPort, OnTestStatusChange);

                    InitializeViews(_qaTestRunManager, _qaTestRunManager.Instrument);
                    ViewContext = EditQaTestViewContext;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                }
                finally
                {
                    ShowConnectionDialog = false;
                }
            }
        }

        /// <summary>
        /// The StartNextTest
        /// </summary>
        /// <returns>The <see cref="Task{object}"/></returns>
        private async Task StartNextTest()
        {
            ShowConnectionDialog = true;

            try
            {
                _qaTestRunManager = await TestRunCreator.CreateNextTestRun(SelectedInstrument, _commPort, OnTestStatusChange);
                InitializeViews(_qaTestRunManager, _qaTestRunManager.Instrument);
                ShowNextTestButton = false;
                ViewContext = EditQaTestViewContext;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
            finally
            {
                ShowConnectionDialog = false;
            }
            
        }

        #endregion

        /// <summary>
        /// Defines the <see cref="SelectableInstrumentType" />
        /// </summary>
        public class SelectableInstrumentType : ReactiveObject
        {
            #region Fields

            /// <summary>
            /// Defines the _isSelected
            /// </summary>
            private bool _isSelected;

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the Instrument
            /// </summary>
            public InstrumentType Instrument { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether IsSelected
            /// </summary>
            public bool IsSelected
            {
                get { return _isSelected; }
                set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
            }

            #endregion
        }
    }
}
//public override void CanClose(Action<bool> callback)
//{
//    base.CanClose(callback);
//    if (_qaRunTestManager != null)
//    {
//        if (!_qaRunTestManager.Instrument.HasPassed)
//        {
//            var result = MessageBox.Show("Instrument test hasn't passed. Would you like to continue?", "Error",
//                MessageBoxButton.YesNo, MessageBoxImage.Warning);
//            if (result == MessageBoxResult.No)
//                callback(false);
//        }
//    }
//}