using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Storage;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Modules.QAProver.Screens.PTVerificationViews;
using ReactiveUI;
using Splat;

namespace Prover.GUI.Modules.QAProver.Screens
{
    public class TestRunViewModel : ViewModelBase, IDisposable
    {
        private const string NewQaTestViewContext = "NewTestView";
        private const string EditQaTestViewContext = "EditTestView";

        private string _connectionStatusMessage;
        private ReactiveList<SelectableInstrumentType> _instrumentTypes;

        private IQaRunTestManager _qaRunTestManager;

        private int _selectedBaudRate;
        private string _selectedCommPort;
        private string _selectedTachCommPort;
        private bool _showConnectionDialog;
        private string _viewContext;
        private CancellationTokenSource _cancellationTokenSource;

        private IDisposable _testStatusSubscription;

        public TestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Client> clientStore)
            : base(screenManager, eventAggregator)
        {
            eventAggregator.Subscribe(this);

            /***  Setup Instruments list  ***/
            InstrumentTypes = new ReactiveList<SelectableInstrumentType>
            {
                ChangeTrackingEnabled = true
            };

            foreach (var x in Instruments.GetAll())
                InstrumentTypes.Add(new SelectableInstrumentType
                {
                    Instrument = x,
                    IsSelected = x.Name == SettingsManager.SettingsInstance.LastInstrumentTypeUsed
                });

            InstrumentTypes.ItemChanged
                .Where(x => x.PropertyName == "IsSelected" && x.Sender.IsSelected)
                .Select(x => x.Sender)
                .Subscribe(async x =>
                {
                    SettingsManager.SettingsInstance.LastInstrumentTypeUsed = x.Instrument.Name;
                    await SettingsManager.Save();
                });

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
                    await SettingsManager.Save();
                });

            /*** Commands ***/
            var canStartNewTest = this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort,
                x => x.SelectedTachCommPort,
                (baud, instrumentPort, tachPort) =>
                    BaudRate.Contains(baud) && !string.IsNullOrEmpty(instrumentPort) &&
                    !string.IsNullOrEmpty(tachPort));

            StartTestCommand = ReactiveCommand.CreateFromTask(StartNewQaTest, canStartNewTest);
            CancelCommand = ReactiveCommand.Create(Cancel);

            _clientList = clientStore.Query().ToList();

            Clients = new ReactiveList<string>(_clientList.Select(x => x.Name).OrderBy(x => x).ToList());
            this.WhenAnyValue(x => x.SelectedClient)
                .Subscribe(_ => { _client = _clientList.FirstOrDefault(x => x.Name == SelectedClient); });

            _viewContext = NewQaTestViewContext;
        }

        public ReactiveCommand StartTestCommand { get; }
        public ReactiveCommand CancelCommand { get; }


        public InstrumentInfoViewModel SiteInformationItem { get; set; }

        public ObservableCollection<VerificationSetViewModel> TestViews { get; set; } =
            new ObservableCollection<VerificationSetViewModel>();

        public VolumeTestViewModel VolumeInformationItem { get; set; }

        public InstrumentInfoViewModel EventLogCommPortItem { get; set; }

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

        public void Dispose()
        {
            _testStatusSubscription?.Dispose();
            _qaRunTestManager?.Dispose();
        }

        public async Task Cancel()
        {
            await ScreenManager.GoHome();
        }

        private async Task StartNewQaTest()
        {
            ShowConnectionDialog = true;

            if (SelectedInstrument != null)
            {
                _cancellationTokenSource = new CancellationTokenSource();

                try
                {
                    SettingsManager.SettingsInstance.LastInstrumentTypeUsed = SelectedInstrument.Name;
                    await SettingsManager.Save();

                    _qaRunTestManager = Locator.Current.GetService<IQaRunTestManager>();
                    _testStatusSubscription = _qaRunTestManager.TestStatus.Subscribe(OnTestStatusChange);
                    await _qaRunTestManager.InitializeTest(SelectedInstrument, _cancellationTokenSource.Token, _client);

                    await InitializeViews(_qaRunTestManager, _qaRunTestManager.Instrument);
                    ViewContext = EditQaTestViewContext;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
                finally
                {
                    ShowConnectionDialog = false;
                }
            }
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

                if (instrument.InstrumentType == Instruments.MiniAt)
                    EventLogCommPortItem = SiteInformationItem;
            });
        }

        public string ViewContext
        {
            get { return _viewContext; }
            set { this.RaiseAndSetIfChanged(ref _viewContext, value); }
        }

        public InstrumentType SelectedInstrument => InstrumentTypes?.FirstOrDefault(i => i.IsSelected)?.Instrument;

        public ReactiveList<SelectableInstrumentType> InstrumentTypes
        {
            get { return _instrumentTypes; }
            set { this.RaiseAndSetIfChanged(ref _instrumentTypes, value); }
        }


        private string _selectedClient;
        private Client _client;
        private readonly List<Client> _clientList;

        private ReactiveList<string> _clients;

        public ReactiveList<string> Clients
        {
            get { return _clients; }
            set { this.RaiseAndSetIfChanged(ref _clients, value); }
        }

        public string SelectedClient
        {
            get { return _selectedClient; }
            set { this.RaiseAndSetIfChanged(ref _selectedClient, value); }
        }

        public List<string> CommPort => SerialPort.GetPortNames().ToList();

        public string SelectedCommPort
        {
            get { return _selectedCommPort; }
            set { this.RaiseAndSetIfChanged(ref _selectedCommPort, value); }
        }

        public List<string> TachCommPort => SerialPort.GetPortNames().ToList();

        public string SelectedTachCommPort
        {
            get { return _selectedTachCommPort; }
            set { this.RaiseAndSetIfChanged(ref _selectedTachCommPort, value); }
        }

        public List<int> BaudRate => CommProtocol.Common.IO.SerialPort.BaudRates;

        public int SelectedBaudRate
        {
            get { return _selectedBaudRate; }
            set { this.RaiseAndSetIfChanged(ref _selectedBaudRate, value); }
        }

        public bool ShowConnectionDialog
        {
            get { return _showConnectionDialog; }
            set { this.RaiseAndSetIfChanged(ref _showConnectionDialog, value); }
        }

        public string ConnectionStatusMessage
        {
            get { return _connectionStatusMessage; }
            set { this.RaiseAndSetIfChanged(ref _connectionStatusMessage, value); }
        }

        public class SelectableInstrumentType
        {
            public InstrumentType Instrument { get; set; }
            public bool IsSelected { get; set; }
        }

        private void OnTestStatusChange(string status)
        {
            ConnectionStatusMessage = status;
        }
    }
}