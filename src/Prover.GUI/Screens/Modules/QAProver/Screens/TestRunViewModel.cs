﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.Settings;
using Prover.Core.Shared.Extensions;
using Prover.Core.VerificationTests;
using Prover.GUI.Events;
using Prover.GUI.Reports;
using Prover.GUI.Screens.Dialogs;
using Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.QAProver.Screens
{
    public class TestRunViewModel : ViewModelBase, IHandle<VerificationTestEvent>
    {
        private readonly InstrumentReportGenerator _instrumentReportGenerator;
        private readonly ISettingsService _settingsService;
        private const string NewQaTestViewContext = "NewTestView";
        private const string EditQaTestViewContext = "EditTestViewNew";
        private IQaRunTestManager _qaRunTestManager;
        private int _selectedBaudRate;
        private string _selectedCommPort;
        private string _selectedTachCommPort;
        private string _viewContext;

        public TestRunViewModel(ScreenManager screenManager, 
            IEventAggregator eventAggregator,
            IClientService clientService, 
            InstrumentReportGenerator instrumentReportGenerator,
            ISettingsService settingsService) 
            : base(screenManager, eventAggregator)
        {
            _instrumentReportGenerator = instrumentReportGenerator;
            _settingsService = settingsService;
            eventAggregator.Subscribe(this);

            RefreshCommPortsCommand = ReactiveCommand.Create(() => SerialPort.GetPortNames().ToList());
            var commPorts = RefreshCommPortsCommand
                .Select(list => new ReactiveList<string>(list));
            commPorts
                .ToProperty(this, x => x.CommPorts, out _commPorts, new ReactiveList<string>() { ChangeTrackingEnabled = true});
            commPorts
                .ToProperty(this, x => x.TachCommPorts, out _tachCommPorts, new ReactiveList<string>() {ChangeTrackingEnabled = true});

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
                
            #region Instruments

            /***  Setup Instruments list  ***/

            InstrumentTypes.AddRange(HoneywellInstrumentTypes.GetAll());

            _selectedInstrumentType =
                HoneywellInstrumentTypes.GetByName(_settingsService.Local.LastInstrumentTypeUsed);

            this.WhenAnyValue(x => x.SelectedInstrumentType)
                .Subscribe(x => _settingsService.Local.LastInstrumentTypeUsed = x?.Name);        

            _selectedBaudRate = BaudRate.Contains(_settingsService.Local.InstrumentBaudRate)
                ? _settingsService.Local.InstrumentBaudRate
                : -1;

            this.WhenAnyValue(x => x.UseIrDaPort)
                .Select(x => !x)
                .ToProperty(this, x => x.DisableCommPortAndBaudRate, out _disableCommPortAndBaudRate);
            UseIrDaPort = _settingsService.Local.InstrumentUseIrDaPort;

            #endregion

            #region Tachometer

            /**
             * 
             * Tachometer Settings
             *
             **/

            this.WhenAnyValue(x => x.TachIsNotUsed)
                .Select(x => !x)
                .ToProperty(this, x => x.TachDisableCommPort, out _tachDisableCommPort, false);

            TachIsNotUsed = _settingsService.Local.TachIsNotUsed;

            #endregion

            #region Commands

            var canStartNewTest = this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort, x => x.TachIsNotUsed,
                (baud, instrumentPort, tachPort, tachNotUsed) => 
                BaudRate.Contains(baud) && !string.IsNullOrEmpty(instrumentPort) && (tachNotUsed || !string.IsNullOrEmpty(tachPort)));

            StartTestCommand =
                DialogDisplayHelpers.ProgressStatusDialogCommand(EventAggregator, "Starting test...", StartNewQaTest);
            StartTestCommand.ThrownExceptions
                .Subscribe(ex => MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));

            var canSave = this.WhenAnyValue(x => x.IsDirty);
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTest, canSave);

            SaveCommand.IsExecuting
                .StepInterval(TimeSpan.FromSeconds(2))
                .ToProperty(this, x => x.ShowSaveSnackbar, out _showSaveSnackbar);

            PrintReportCommand = ReactiveCommand.CreateFromTask(PrintTest);
            #endregion

            #region Clients

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

            #endregion

            this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort,
                    x => x.SelectedClient, x => x.TachIsNotUsed, x => x.UseIrDaPort, x => x.SelectedInstrumentType)
                .Subscribe( _ =>
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

       

        #region Commands

        public ReactiveCommand SaveCommand { get; }
        public ReactiveCommand PrintReportCommand { get; }
        public ReactiveCommand StartTestCommand { get; }

        private ReactiveCommand<Unit, List<string>> _refreshCommPortsCommand;
        public ReactiveCommand<Unit, List<string>> RefreshCommPortsCommand
        {
            get => _refreshCommPortsCommand;
            set => this.RaiseAndSetIfChanged(ref _refreshCommPortsCommand, value);
        }

        #endregion

        #region Properties

        private readonly ObservableAsPropertyHelper<ReactiveList<string>> _commPorts;
        public ReactiveList<string> CommPorts => _commPorts.Value;
        private readonly ObservableAsPropertyHelper<ReactiveList<string>> _tachCommPorts;
        public ReactiveList<string> TachCommPorts => _tachCommPorts.Value;
        private readonly ObservableAsPropertyHelper<bool> _disableCommPortAndBaudRate;
        public bool DisableCommPortAndBaudRate => _disableCommPortAndBaudRate.Value;
        private readonly ObservableAsPropertyHelper<bool> _showSaveSnackbar;
        public bool ShowSaveSnackbar => _showSaveSnackbar.Value;
        private bool _isDirty;

        public bool IsDirty
        {
            get => _isDirty;
            set => this.RaiseAndSetIfChanged(ref _isDirty, value);
        }

        public bool DisplayHeader => _qaRunTestManager != null;
        public InstrumentInfoViewModel SiteInformationItem { get; set; }

        public ObservableCollection<VerificationSetViewModel> TestViews { get; set; } =
            new ObservableCollection<VerificationSetViewModel>();

        public VolumeTestViewModel VolumeTestView =>
            TestViews.FirstOrDefault(x => x.VolumeTestViewModel != null)?.VolumeTestViewModel;

        public List<VerificationSetViewModel> PTTestViews => TestViews.ToList();
        public VolumeTestViewModel VolumeInformationItem { get; set; }
        public InstrumentInfoViewModel EventLogCommPortItem { get; set; }

        public string ViewContext
        {
            get => _viewContext;
            set => this.RaiseAndSetIfChanged(ref _viewContext, value);
        }

        private InstrumentType _selectedInstrumentType;

        public InstrumentType SelectedInstrumentType
        {
            get => _selectedInstrumentType;
            set => this.RaiseAndSetIfChanged(ref _selectedInstrumentType, value);
        }

        private ReactiveList<InstrumentType> _instrumentTypes =
            new ReactiveList<InstrumentType> {ChangeTrackingEnabled = true};

        public ReactiveList<InstrumentType> InstrumentTypes
        {
            get => _instrumentTypes;
            set => this.RaiseAndSetIfChanged(ref _instrumentTypes, value);
        }

        public string SelectedCommPort
        {
            get => _selectedCommPort;
            set => this.RaiseAndSetIfChanged(ref _selectedCommPort, value);
        }

        public string SelectedTachCommPort
        {
            get => _selectedTachCommPort;
            set => this.RaiseAndSetIfChanged(ref _selectedTachCommPort, value);
        }

        public List<int> BaudRate => CommProtocol.Common.IO.SerialPort.BaudRates;

        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set => this.RaiseAndSetIfChanged(ref _selectedBaudRate, value);
        }

        private bool _showDialog;

        public bool ShowDialog
        {
            get => _showDialog;
            set => this.RaiseAndSetIfChanged(ref _showDialog, value);
        }

        private string _selectedClient;
        private Client _client;
        private ReactiveList<string> _clients;

        public ReactiveList<string> Clients
        {
            get => _clients;
            set => this.RaiseAndSetIfChanged(ref _clients, value);
        }

        public string SelectedClient
        {
            get => _selectedClient;
            set => this.RaiseAndSetIfChanged(ref _selectedClient, value);
        }

        private bool _tachIsNotUsed;

        public bool TachIsNotUsed
        {
            get => _tachIsNotUsed;
            set => this.RaiseAndSetIfChanged(ref _tachIsNotUsed, value);
        }

        private bool _useIrDaPort;

        public bool UseIrDaPort
        {
            get => _useIrDaPort;
            set => this.RaiseAndSetIfChanged(ref _useIrDaPort, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _tachDisableCommPort;
        public bool TachDisableCommPort => _tachDisableCommPort.Value;

        #endregion

        #region Methods

        private async Task PrintTest()
        {
            await _instrumentReportGenerator.GenerateAndViewReport(_qaRunTestManager.Instrument);
        }

        private async Task SaveTest()
        {
            if (_qaRunTestManager != null)
            {
                await _qaRunTestManager.SaveAsync();
                IsDirty = false;
            }
        }

        private async Task StartNewQaTest(IObserver<string> statusObservable, CancellationToken ct)
        {
            if (SelectedInstrumentType == null)
                return;

            ShowDialog = true;

            try
            {
                await _settingsService.SaveLocalSettingsAsync();
 
                _qaRunTestManager = IoC.Get<IQaRunTestManager>();
                await _qaRunTestManager.InitializeTest(SelectedInstrumentType, GetCommPort(), _settingsService.Shared.TestSettings, ct, _client, statusObservable);

                await InitializeViews(_qaRunTestManager, _qaRunTestManager.Instrument);
                ViewContext = EditQaTestViewContext;

                IsDirty = true;
            }
            catch (Exception ex)
            {
                _qaRunTestManager?.Dispose();

                if (ex is OperationCanceledException)
                    Log.Warn("Test init cancelled by user.");
                else
                    throw;
            }
            finally
            {
                ShowDialog = false;
            }
        }

        private ICommPort GetCommPort()
        {
           if (UseIrDaPort)
               return new IrDAPort();

            return new SerialPort(_settingsService.Local.InstrumentCommPort, _settingsService.Local.InstrumentBaudRate);
        }

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

        public void Handle(VerificationTestEvent message)
        {
            IsDirty = true;
        }

        #endregion
    }
}