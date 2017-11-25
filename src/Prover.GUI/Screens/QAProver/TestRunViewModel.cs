using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Akka.Util.Internal;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Events;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver.PTVerificationViews;
using ReactiveUI;
using Splat;

namespace Prover.GUI.Screens.QAProver
{
    public class TestRunViewModel : ViewModelBase, IDisposable, IHandle<ConnectionStatusEvent>, IHandle<SaveTestEvent>
    {
        
        private const string NewQaTestViewContext = "NewTestView";
        private const string EditQaTestViewContext = "EditTestView";

        private string _connectionStatusMessage;
        private ReactiveList<SelectableInstrumentType> _instrumentTypes;

        private IQaRunTestManager _qaTestRunManager;

        private int _selectedBaudRate;
        private string _selectedCommPort;
        private string _selectedTachCommPort;
        private bool _showConnectionDialog;

        private IDisposable _testStatusSubscription;
        private string _viewContext;

        public TestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
           
           

            /***  Setup Instruments list  ***/
            InstrumentTypes = new ReactiveList<SelectableInstrumentType>
            {
                ChangeTrackingEnabled = true
            };

            Instruments.GetAll().ForEach(
                x => InstrumentTypes.Add(new SelectableInstrumentType
                {
                    Instrument = x,
                    IsSelected = x.Name == SettingsManager.SettingsInstance.LastInstrumentTypeUsed
                }));

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

            this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort, x => x.SelectedInstrument)
                .Subscribe(async _ =>
                {
                    SettingsManager.SettingsInstance.InstrumentBaudRate = _.Item1;
                    SettingsManager.SettingsInstance.InstrumentCommPort = _.Item2;
                    SettingsManager.SettingsInstance.TachCommPort = _.Item3;
                    SettingsManager.SettingsInstance.LastInstrumentTypeUsed = _.Item4?.Name;

                    await SettingsManager.Save();
                });

            /*** Commands ***/
            var canStartNewTest = this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort,
                (baud, instrumentPort, tachPort) =>
                    BaudRate.Contains(baud) && !string.IsNullOrEmpty(instrumentPort) && !string.IsNullOrEmpty(tachPort));

            StartTestCommand = ReactiveCommand.CreateFromTask(StartNewQaTest, canStartNewTest);

            _viewContext = NewQaTestViewContext;
        }

        #region Properties
        public ReactiveCommand StartTestCommand { get; }

        public InstrumentInfoViewModel SiteInformationItem { get; set; }

        public ObservableCollection<VerificationSetViewModel> TestViews { get; set; } =
            new ObservableCollection<VerificationSetViewModel>();

        public VolumeTestViewModel VolumeTestView =>
            TestViews.FirstOrDefault(x => x.VolumeTestViewModel != null)?.VolumeTestViewModel;

        public InstrumentInfoViewModel EventLogCommPortItem { get; set; }

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
#endregion

        public void Handle(ConnectionStatusEvent message)
        {
            ConnectionStatusMessage = $"Attempt {message.ConnectionStatus} of {message.MaxAttempts}...";
        }

        public async Task CancelCommand()
        {
            await ScreenManager.GoHome();
        }

        private async Task StartNewQaTest()
        {
            ShowConnectionDialog = true;

            if (SelectedInstrument != null)
                try
                {
                    var commPort = GetCommPort();
                    _qaTestRunManager = Locator.Current.GetService<IQaRunTestManager>();
                    _testStatusSubscription = _qaTestRunManager.TestStatus.Subscribe(OnTestStatusChange);
                    await _qaTestRunManager.InitializeTest(SelectedInstrument, commPort);
                    await InitializeViews(_qaTestRunManager, _qaTestRunManager.Instrument);
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

        private static CommPort GetCommPort()
        {
            return new SerialPort(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate);
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

        private void OnTestStatusChange(string status)
        {
            ConnectionStatusMessage = status;
        }

        public class SelectableInstrumentType
        {
            public InstrumentType Instrument { get; set; }
            public bool IsSelected { get; set; }
        }

        public void Dispose()
        {
            _testStatusSubscription?.Dispose();
            _qaTestRunManager?.Dispose();
        }

        public void Handle(SaveTestEvent message)
        {
            _qaTestRunManager?.SaveAsync();
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