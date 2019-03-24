namespace Prover.GUI.Screens.Modules.QAProver.Screens
{
    using Caliburn.Micro;
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.CommProtocol.MiHoneywell;
    using Prover.Core.Models.Clients;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Services;
    using Prover.Core.Settings;
    using Prover.Core.Shared.Extensions;
    using Prover.Core.Testing;
    using Prover.Core.VerificationTests;
    using Prover.GUI.Events;
    using Prover.GUI.Reports;
    using Prover.GUI.Screens.Dialogs;
    using Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Defines the <see cref="TestRunViewModel" />
    /// </summary>
    public class TestRunViewModel : ViewModelBase, IHandle<VerificationTestEvent>
    {
        #region Constants

        /// <summary>
        /// Defines the EditQaTestViewContext
        /// </summary>
        private const string EditQaTestViewContext = "EditTestViewNew";

        /// <summary>
        /// Defines the NewQaTestViewContext
        /// </summary>
        private const string NewQaTestViewContext = "NewTestView";

        #endregion

        #region Fields

        /// <summary>
        /// Defines the _commPorts
        /// </summary>
        private readonly ObservableAsPropertyHelper<ReactiveList<string>> _commPorts;

        /// <summary>
        /// Defines the _disableCommPortAndBaudRate
        /// </summary>
        private readonly ObservableAsPropertyHelper<bool> _disableCommPortAndBaudRate;

        /// <summary>
        /// Defines the _instrumentReportGenerator
        /// </summary>
        private readonly InstrumentReportGenerator _instrumentReportGenerator;

        /// <summary>
        /// Defines the _settingsService
        /// </summary>
        private readonly ISettingsService _settingsService;
        private readonly RotaryStressTest _rotaryStressTest;

        /// <summary>
        /// Defines the _showSaveSnackbar
        /// </summary>
        private readonly ObservableAsPropertyHelper<bool> _showSaveSnackbar;

        /// <summary>
        /// Defines the _tachCommPorts
        /// </summary>
        private readonly ObservableAsPropertyHelper<ReactiveList<string>> _tachCommPorts;

        /// <summary>
        /// Defines the _tachDisableCommPort
        /// </summary>
        private readonly ObservableAsPropertyHelper<bool> _tachDisableCommPort;

        /// <summary>
        /// Defines the _client
        /// </summary>
        private Client _client;

        /// <summary>
        /// Defines the _clients
        /// </summary>
        private ReactiveList<string> _clients;

        /// <summary>
        /// Defines the _instrumentTypes
        /// </summary>
        private ReactiveList<InstrumentType> _instrumentTypes =
            new ReactiveList<InstrumentType> { ChangeTrackingEnabled = true };

        /// <summary>
        /// Defines the _isDirty
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// Defines the _isLoading
        /// </summary>
        private bool _isLoading;

        /// <summary>
        /// Defines the _qaRunTestManager
        /// </summary>
        private IQaRunTestManager _qaRunTestManager;

        /// <summary>
        /// Defines the _refreshCommPortsCommand
        /// </summary>
        private ReactiveCommand<Unit, List<string>> _refreshCommPortsCommand;

        /// <summary>
        /// Defines the _selectedBaudRate
        /// </summary>
        private int _selectedBaudRate;

        /// <summary>
        /// Defines the _selectedClient
        /// </summary>
        private string _selectedClient;

        /// <summary>
        /// Defines the _selectedCommPort
        /// </summary>
        private string _selectedCommPort;

        /// <summary>
        /// Defines the _selectedInstrumentType
        /// </summary>
        private InstrumentType _selectedInstrumentType;

        /// <summary>
        /// Defines the _selectedTachCommPort
        /// </summary>
        private string _selectedTachCommPort;

        /// <summary>
        /// Defines the _showDialog
        /// </summary>
        private bool _showDialog;

        /// <summary>
        /// Defines the _tachIsNotUsed
        /// </summary>
        private bool _tachIsNotUsed;

        /// <summary>
        /// Defines the _useIrDaPort
        /// </summary>
        private bool _useIrDaPort;

        /// <summary>
        /// Defines the _viewContext
        /// </summary>
        private string _viewContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRunViewModel"/> class.
        /// </summary>
        /// <param name="screenManager">The screenManager<see cref="ScreenManager"/></param>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="clientService">The clientService<see cref="IClientService"/></param>
        /// <param name="instrumentReportGenerator">The instrumentReportGenerator<see cref="InstrumentReportGenerator"/></param>
        /// <param name="settingsService">The settingsService<see cref="ISettingsService"/></param>
        public TestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, IClientService clientService,
            InstrumentReportGenerator instrumentReportGenerator, ISettingsService settingsService, RotaryStressTest rotaryStressTest)
            : base(screenManager, eventAggregator)
        {
            _instrumentReportGenerator = instrumentReportGenerator;
            _settingsService = settingsService;
            _rotaryStressTest = rotaryStressTest;
            eventAggregator.Subscribe(this);

            RefreshCommPortsCommand = ReactiveCommand.Create(() => SerialPort.GetPortNames().ToList());
            var commPorts = RefreshCommPortsCommand
                .Select(list => new ReactiveList<string>(list));
            commPorts
                .ToProperty(this, x => x.CommPorts, out _commPorts, new ReactiveList<string>() { ChangeTrackingEnabled = true });
            commPorts
                .ToProperty(this, x => x.TachCommPorts, out _tachCommPorts, new ReactiveList<string>() { ChangeTrackingEnabled = true });

            SelectedCommPort = _settingsService.Local.InstrumentCommPort;
            SelectedTachCommPort = _settingsService.Local.TachCommPort;

            commPorts.Subscribe(list =>
            {
                SelectedCommPort = list.Contains(_settingsService.Local.InstrumentCommPort)
                    ? _settingsService.Local.InstrumentCommPort
                    : string.Empty;

                SelectedTachCommPort = list.Contains(_settingsService.Local.TachCommPort)
                    ? _settingsService.Local.TachCommPort
                    : string.Empty;
            });

            /***  Setup Instruments list  ***/

            InstrumentTypes.AddRange(HoneywellInstrumentTypes.GetAll());

            _selectedInstrumentType =
                HoneywellInstrumentTypes.GetByName(_settingsService.Local.LastInstrumentTypeUsed);

            this.WhenAnyValue(x => x.SelectedInstrumentType)
                .Subscribe(x => _settingsService.Local.LastInstrumentTypeUsed = x?.Name);

            this.WhenAnyValue(x => x.SelectedInstrumentType)
                .Where(i => i != null)
                .Subscribe(i =>
                {
                    UseIrDaPort = i?.CanUseIrDaPort == true;
                    if (i.MaxBaudRate.HasValue)
                    {
                        SelectedBaudRate = i.MaxBaudRate.Value;
                    }
                });

            _selectedBaudRate = BaudRate.Contains(_settingsService.Local.InstrumentBaudRate)
                ? _settingsService.Local.InstrumentBaudRate
                : -1;

            this.WhenAnyValue(x => x.UseIrDaPort)
                .Select(x => !x)
                .ToProperty(this, x => x.DisableCommPortAndBaudRate, out _disableCommPortAndBaudRate);
            UseIrDaPort = _settingsService.Local.InstrumentUseIrDaPort;

            /**
             * 
             * Tachometer Settings
             *
             **/

            this.WhenAnyValue(x => x.TachIsNotUsed)
                .Select(x => !x)
                .ToProperty(this, x => x.TachDisableCommPort, out _tachDisableCommPort, false);

            TachIsNotUsed = _settingsService.Local.TachIsNotUsed;

            var canStartInstrument = this.WhenAnyValue(x => x.SelectedInstrumentType)
                .Select(i => i != null);

            var canStartCommPort = this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.UseIrDaPort,
                (baud, port, irda) => (BaudRate.Contains(baud) && !string.IsNullOrEmpty(port)) || irda);

            var canStartTach = this.WhenAnyValue(x => x.SelectedTachCommPort, x => x.TachIsNotUsed,
                (tachPort, tachNotUsed) => tachNotUsed || !string.IsNullOrEmpty(tachPort));

            var canStartNewTest = Observable.Merge(new [] { canStartInstrument, canStartCommPort, canStartTach });

            StartTestCommand =
                DialogDisplayHelpers.ProgressStatusDialogCommand(EventAggregator, "Starting test...", StartNewQaTest, canStartNewTest);
            StartTestCommand.ThrownExceptions
                .Subscribe(ex => MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));

            StartRotarySmokeTestCommand =
                DialogDisplayHelpers.ProgressStatusDialogCommand(EventAggregator, "Running Smoke test...", StartRotarySmokeTest, canStartNewTest);

            var canSave = this.WhenAnyValue(x => x.IsDirty);
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTest, canSave);

            SaveCommand.IsExecuting
                .StepInterval(TimeSpan.FromSeconds(2))
                .ToProperty(this, x => x.ShowSaveSnackbar, out _showSaveSnackbar);

            /** Auto Save logic*/
            //this.WhenAnyValue(x => x.IsDirty)
            //     .Where(dirty => dirty && !_isLoading && _settingsService.Local.AutoSave)   
            //     .Select(x => new Unit())                                 
            //     .InvokeCommand(this, x => x.SaveCommand);

            PrintReportCommand = ReactiveCommand.CreateFromTask(PrintTest);

            /**             
            * Clients              
            **/
            var clientList = clientService.GetActiveClients()
                .ToList();
            Clients = new ReactiveList<string>(
                clientList.Select(x => x.Name).OrderBy(x => x).ToList())
            {
                string.Empty
            };

            this.WhenAnyValue(x => x.SelectedClient)
                .Subscribe(_ => { _client = clientList.FirstOrDefault(x => x.Name == SelectedClient); });
            SelectedClient = Clients.Contains(_settingsService.Local.LastClientSelected)
                ? _settingsService.Local.LastClientSelected
                : string.Empty;

            this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort,
                    x => x.SelectedClient, x => x.TachIsNotUsed, x => x.UseIrDaPort, x => x.SelectedInstrumentType)
                .Subscribe(_ =>
               {
                   _settingsService.Local.InstrumentBaudRate = _.Item1;
                   _settingsService.Local.InstrumentCommPort = _.Item2;
                   _settingsService.Local.TachCommPort = _.Item3;
                   _settingsService.Local.LastClientSelected = _.Item4;
                   _settingsService.Local.TachIsNotUsed = _.Item5;
                   _settingsService.Local.InstrumentUseIrDaPort = _.Item6;
                   _settingsService.Local.LastInstrumentTypeUsed = _.Item7?.Name;
               });

            _viewContext = NewQaTestViewContext;
        }

       

        #endregion

        #region Properties

        /// <summary>
        /// Gets the BaudRate
        /// </summary>
        public List<int> BaudRate => CommProtocol.Common.IO.SerialPort.BaudRates;

        /// <summary>
        /// Gets or sets the Clients
        /// </summary>
        public ReactiveList<string> Clients { get => _clients; set => this.RaiseAndSetIfChanged(ref _clients, value); }

        /// <summary>
        /// Gets the CommPorts
        /// </summary>
        public ReactiveList<string> CommPorts => _commPorts.Value;

        /// <summary>
        /// Gets a value indicating whether DisableCommPortAndBaudRate
        /// </summary>
        public bool DisableCommPortAndBaudRate => _disableCommPortAndBaudRate.Value;

        /// <summary>
        /// Gets a value indicating whether DisplayHeader
        /// </summary>
        public bool DisplayHeader => _qaRunTestManager != null;

        /// <summary>
        /// Gets or sets the EventLogCommPortItem
        /// </summary>
        public InstrumentInfoViewModel EventLogCommPortItem { get; set; }

        /// <summary>
        /// Gets or sets the InstrumentTypes
        /// </summary>
        public ReactiveList<InstrumentType> InstrumentTypes { get => _instrumentTypes; set => this.RaiseAndSetIfChanged(ref _instrumentTypes, value); }

        /// <summary>
        /// Gets or sets a value indicating whether IsDirty
        /// </summary>
        public bool IsDirty { get => _isDirty; set => this.RaiseAndSetIfChanged(ref _isDirty, value); }

        /// <summary>
        /// Gets the PrintReportCommand
        /// </summary>
        public ReactiveCommand PrintReportCommand { get; }

        /// <summary>
        /// Gets the PTTestViews
        /// </summary>
        public List<VerificationSetViewModel> PTTestViews => TestViews.ToList();

        /// <summary>
        /// Gets or sets the RefreshCommPortsCommand
        /// </summary>
        public ReactiveCommand<Unit, List<string>> RefreshCommPortsCommand { get => _refreshCommPortsCommand; set => this.RaiseAndSetIfChanged(ref _refreshCommPortsCommand, value); }

        /// <summary>
        /// Gets the SaveCommand
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        /// <summary>
        /// Gets or sets the SelectedBaudRate
        /// </summary>
        public int SelectedBaudRate { get => _selectedBaudRate; set => this.RaiseAndSetIfChanged(ref _selectedBaudRate, value); }

        /// <summary>
        /// Gets or sets the SelectedClient
        /// </summary>
        public string SelectedClient { get => _selectedClient; set => this.RaiseAndSetIfChanged(ref _selectedClient, value); }

        /// <summary>
        /// Gets or sets the SelectedCommPort
        /// </summary>
        public string SelectedCommPort { get => _selectedCommPort; set => this.RaiseAndSetIfChanged(ref _selectedCommPort, value); }

        /// <summary>
        /// Gets or sets the SelectedInstrumentType
        /// </summary>
        public InstrumentType SelectedInstrumentType { get => _selectedInstrumentType; set => this.RaiseAndSetIfChanged(ref _selectedInstrumentType, value); }

        /// <summary>
        /// Gets or sets the SelectedTachCommPort
        /// </summary>
        public string SelectedTachCommPort { get => _selectedTachCommPort; set => this.RaiseAndSetIfChanged(ref _selectedTachCommPort, value); }

        /// <summary>
        /// Gets or sets a value indicating whether ShowDialog
        /// </summary>
        public bool ShowDialog { get => _showDialog; set => this.RaiseAndSetIfChanged(ref _showDialog, value); }

        /// <summary>
        /// Gets a value indicating whether ShowSaveSnackbar
        /// </summary>
        public bool ShowSaveSnackbar => _showSaveSnackbar.Value;

        /// <summary>
        /// Gets or sets the SiteInformationItem
        /// </summary>
        public InstrumentInfoViewModel SiteInformationItem { get; set; }

        /// <summary>
        /// Gets the StartTestCommand
        /// </summary>
        public ReactiveCommand StartTestCommand { get; }

        /// <summary>
        /// Gets the TachCommPorts
        /// </summary>
        public ReactiveList<string> TachCommPorts => _tachCommPorts.Value;

        /// <summary>
        /// Gets a value indicating whether TachDisableCommPort
        /// </summary>
        public bool TachDisableCommPort => _tachDisableCommPort.Value;

        /// <summary>
        /// Gets or sets a value indicating whether TachIsNotUsed
        /// </summary>
        public bool TachIsNotUsed { get => _tachIsNotUsed; set => this.RaiseAndSetIfChanged(ref _tachIsNotUsed, value); }

        /// <summary>
        /// Gets or sets the TestViews
        /// </summary>
        public ObservableCollection<VerificationSetViewModel> TestViews { get; set; } =
            new ObservableCollection<VerificationSetViewModel>();

        /// <summary>
        /// Gets or sets a value indicating whether UseIrDaPort
        /// </summary>
        public bool UseIrDaPort { get => _useIrDaPort; set => this.RaiseAndSetIfChanged(ref _useIrDaPort, value); }

        /// <summary>
        /// Gets or sets the ViewContext
        /// </summary>
        public string ViewContext { get => _viewContext; set => this.RaiseAndSetIfChanged(ref _viewContext, value); }

        /// <summary>
        /// Gets or sets the VolumeInformationItem
        /// </summary>
        public VolumeTestViewModel VolumeInformationItem { get; set; }

        /// <summary>
        /// Gets the VolumeTestView
        /// </summary>
        public VolumeTestViewModel VolumeTestView =>
            TestViews.FirstOrDefault(x => x.VolumeTestViewModel != null)?.VolumeTestViewModel;

        public ReactiveCommand StartRotarySmokeTestCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// The CanClose
        /// </summary>
        /// <param name="callback">The callback<see cref="Action{bool}"/></param>
        public override void CanClose(Action<bool> callback)
        {
            if (_qaRunTestManager?.Instrument != null)
            {
                MessageBoxResult result;
                if (!_qaRunTestManager.Instrument.HasPassed)
                {
                    result = MessageBox.Show($"Instrument hasn't passed all tests. {Environment.NewLine}" +
                                             $"Continue anyways?", "Failed Tests",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        callback(false);
                        return;
                    }
                }

                if (IsDirty)
                {
                    result = MessageBox.Show($"Save changes?", "Save", MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                        SaveTest().ConfigureAwait(false);

                    if (result == MessageBoxResult.Cancel)
                    {
                        callback(false);
                        return;
                    }
                }
            }

            callback(true);
        }

        /// <summary>
        /// The Dispose
        /// </summary>
        public override void Dispose()
        {
            if (ViewContext == EditQaTestViewContext)
            {
                foreach (var testView in TestViews)
                {
                    testView.Dispose();
                    testView.TryClose();
                }
                SiteInformationItem = null;
                _qaRunTestManager?.Dispose();
            }
        }

        /// <summary>
        /// The Handle
        /// </summary>
        /// <param name="message">The message<see cref="VerificationTestEvent"/></param>
        public void Handle(VerificationTestEvent message)
        {
            IsDirty = true;
            this.WhenAnyValue(x => x.SaveCommand)
                .Where(x => !_isLoading && _settingsService.Local.AutoSave)
                .SelectMany(x => x.Execute())
                .Subscribe();
        }

        /// <summary>
        /// The InitializeViews
        /// </summary>
        /// <param name="qaTestRunTestManager">The qaTestRunTestManager<see cref="IQaRunTestManager"/></param>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task InitializeViews(IQaRunTestManager qaTestRunTestManager, Instrument instrument)
        {
            await Task.Run(() =>
            {
                SiteInformationItem = ScreenManager.ResolveViewModel<InstrumentInfoViewModel>();
                SiteInformationItem.QaTestManager = qaTestRunTestManager;
                SiteInformationItem.Instrument = instrument;

                foreach (var x in instrument.VerificationTests.OrderBy(v => v.TestNumber))
                {
                    var item = ScreenManager.ResolveViewModel<VerificationSetViewModel>();
                    item.InitializeViews(x, qaTestRunTestManager);
                    item.VerificationTest = x;

                    TestViews.Add(item);
                }
            });
        }

        /// <summary>
        /// The GetCommPort
        /// </summary>
        /// <returns>The <see cref="ICommPort"/></returns>
        private ICommPort GetCommPort()
        {
            if (UseIrDaPort)
                return new IrDAPort();

            return new SerialPort(_settingsService.Local.InstrumentCommPort, _settingsService.Local.InstrumentBaudRate);
        }

        /// <summary>
        /// The PrintTest
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task PrintTest()
        {
            await _instrumentReportGenerator.GenerateAndViewReport(_qaRunTestManager.Instrument);
        }

        /// <summary>
        /// The SaveTest
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task SaveTest()
        {
            if (IsDirty && _qaRunTestManager != null)
            {
                await _qaRunTestManager?.SaveAsync();
                IsDirty = false;
            }
        }

        /// <summary>
        /// The StartNewQaTest
        /// </summary>
        /// <param name="statusObservable">The statusObservable<see cref="IObserver{string}"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task StartNewQaTest(IObserver<string> statusObservable, CancellationToken ct)
        {
            if (SelectedInstrumentType == null)
                return;

            ShowDialog = true;
            _isLoading = true;

            try
            {
                await _settingsService.SaveSettings();

                _qaRunTestManager = IoC.Get<IQaRunTestManager>();
                _qaRunTestManager.Status.Subscribe(statusObservable);
                await _qaRunTestManager.InitializeTest(SelectedInstrumentType, GetCommPort(), _settingsService, ct, _client);                

                await InitializeViews(_qaRunTestManager, _qaRunTestManager.Instrument);
                ViewContext = EditQaTestViewContext;

                IsDirty = true;
                if (_settingsService.Local.AutoSave) await SaveTest().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _qaRunTestManager?.Dispose();

                if (ex is OperationCanceledException)
                    Log.Warn("Test init cancelled by user.");
                else
                    Log.Error(ex);
            }
            finally
            {
                ShowDialog = false;
                _isLoading = false;
            }
        }
        private async Task StartRotarySmokeTest(IObserver<string> statusObservable, CancellationToken ct)
        {
            try
            {
                if (SelectedInstrumentType == null)
                    return;

                ShowDialog = true;
                _isLoading = true;

                await _settingsService.SaveSettings();

                _rotaryStressTest.Status.Subscribe(statusObservable);
                await _rotaryStressTest.Run(SelectedInstrumentType, _client, ct);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                ShowDialog = false;
                _isLoading = false;
            }          

        }
        #endregion
    }
}
