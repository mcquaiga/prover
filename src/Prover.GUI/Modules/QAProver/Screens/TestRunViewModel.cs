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
using Prover.GUI.Common.Screens.Dialogs;
using Prover.GUI.Modules.QAProver.Screens.PTVerificationViews;
using ReactiveUI;
using Splat;

namespace Prover.GUI.Modules.QAProver.Screens
{
    public class TestRunViewModel : ViewModelBase, IDisposable
    {
        private const string NewQaTestViewContext = "NewTestView";
        private const string EditQaTestViewContext = "EditTestViewNew";
        private ReactiveList<SelectableInstrumentType> _instrumentTypes;
        private IQaRunTestManager _qaRunTestManager;
        private int _selectedBaudRate;
        private string _selectedCommPort;
        private string _selectedTachCommPort;
        private string _viewContext;

        public TestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IClientStore clientStore)
            : base(screenManager, eventAggregator)
        {
            eventAggregator.Subscribe(this);

            /***  Setup Instruments list  ***/
            InstrumentTypes = new ReactiveList<SelectableInstrumentType>
            {
                ChangeTrackingEnabled = true
            };

            foreach (var x in HoneywellInstrumentTypes.GetAll())
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

            /*** Commands ***/
            var canStartNewTest = this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort,
                x => x.SelectedTachCommPort, x => x.TachIsNotUsed,
                (baud, instrumentPort, tachPort, tachNotUsed) =>
                    BaudRate.Contains(baud) && !string.IsNullOrEmpty(instrumentPort) &&
                    (tachNotUsed || !string.IsNullOrEmpty(tachPort)));

            StartTestCommand = ReactiveCommand.CreateFromTask(async () => await ScreenManager.ShowModalDialog(
                new ProgressStatusDialogViewModel("Starting test...", StartNewQaTest)));

            //StartTestCommand =
            //    DialogDisplayHelpers.ProgressStatusDialogCommand(EventAggregator, "Starting test...", StartNewQaTest);

            var clientList = clientStore.Query().ToList();
            Clients = new ReactiveList<string>(
                clientList.Select(x => x.Name).OrderBy(x => x).ToList())
            {
                string.Empty
            };

            this.WhenAnyValue(x => x.SelectedClient)
                .Subscribe(_ => { _client = clientList.FirstOrDefault(x => x.Name == SelectedClient); });
            SelectedClient = Clients.Contains(SettingsManager.SettingsInstance.LastClientSelected)
                ? SettingsManager.SettingsInstance.LastClientSelected
                : string.Empty;

            this.WhenAnyValue(x => x.TachIsNotUsed)
                .Select(x => !x)
                .ToProperty(this, x => x.TachDisableCommPort, out _tachDisableCommPort, false);

            TachIsNotUsed = SettingsManager.SettingsInstance.TachIsNotUsed;
            this.WhenAnyValue(x => x.TachIsNotUsed)
                .Subscribe(t => SettingsManager.SettingsInstance.TachIsNotUsed = t);

            this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort,
                    x => x.SelectedClient, x => x.TachIsNotUsed)
                .Subscribe(
                    async _ =>
                    {
                        SettingsManager.SettingsInstance.InstrumentBaudRate = _.Item1;
                        SettingsManager.SettingsInstance.InstrumentCommPort = _.Item2;
                        SettingsManager.SettingsInstance.TachCommPort = _.Item3;
                        SettingsManager.SettingsInstance.LastClientSelected = _.Item4;
                        
                        await SettingsManager.Save();
                    });

            _viewContext = NewQaTestViewContext;
        }

        #region Properties

        public ReactiveCommand StartTestCommand { get; }
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

        public InstrumentType SelectedInstrument => InstrumentTypes?.FirstOrDefault(i => i.IsSelected)?.Instrument;

        public ReactiveList<SelectableInstrumentType> InstrumentTypes
        {
            get => _instrumentTypes;
            set => this.RaiseAndSetIfChanged(ref _instrumentTypes, value);
        }

        public List<string> CommPort => SerialPort.GetPortNames().ToList();

        public string SelectedCommPort
        {
            get => _selectedCommPort;
            set => this.RaiseAndSetIfChanged(ref _selectedCommPort, value);
        }

        public List<string> TachCommPort => SerialPort.GetPortNames().ToList();

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

        private readonly ObservableAsPropertyHelper<bool> _tachDisableCommPort;
        public bool TachDisableCommPort => _tachDisableCommPort.Value;

        #endregion

        public async Task Cancel()
        {
            await ScreenManager.GoHome();
        }

        private async Task StartNewQaTest(IObserver<string> statusObservable, CancellationToken ct)
        {
            if (SelectedInstrument == null)
                return;

            ShowDialog = true;

            try
            {
                SettingsManager.SettingsInstance.LastInstrumentTypeUsed = SelectedInstrument?.Name;
                await SettingsManager.Save();

                _qaRunTestManager = Locator.Current.GetService<IQaRunTestManager>();
                _qaRunTestManager.TestStatus.Subscribe(statusObservable);

                await _qaRunTestManager.InitializeTest(SelectedInstrument, ct, _client);

                await InitializeViews(_qaRunTestManager, _qaRunTestManager.Instrument);
                ViewContext = EditQaTestViewContext;
            }
            catch (Exception ex)
            {
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

                if (instrument.InstrumentType == HoneywellInstrumentTypes.MiniAt)
                    EventLogCommPortItem = SiteInformationItem;
            });
        }

        public class SelectableInstrumentType
        {
            public InstrumentType Instrument { get; set; }
            public bool IsSelected { get; set; }
        }

        public void Dispose()
        {
            //_testStatusSubscription?.Dispose();
            _qaRunTestManager?.Dispose();
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