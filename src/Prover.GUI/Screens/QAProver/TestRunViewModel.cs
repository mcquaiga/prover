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
using Prover.Core.Settings;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver.VerificationTestViews;
using Prover.GUI.Screens.Settings;
using ReactiveUI;
using SerialPort = System.IO.Ports.SerialPort;

namespace Prover.GUI.Screens.QAProver
{
    public class TestRunViewModel : ViewModelBase
    {
        private const string NewQaTestViewContext = "NewTestView";
        private const string EditQaTestViewContext = "Edit";

        public ReactiveCommand<object> StartTestCommand { get; private set; }

        public TestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            SettingsManager.RefreshSettings();

            /***  Setup Instruments list  ***/
            InstrumentTypes = new ReactiveList<SelectableInstrumentType>
            {
                ChangeTrackingEnabled = true,
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
                .Subscribe(x => {
                    SettingsManager.SettingsInstance.LastInstrumentTypeUsed = x.Instrument.Name;
                    SettingsManager.Save();
                });

            /***  Setup Comm Ports and Baud Rate settings ***/
            CommPort = PortsWatcherObservable().CreateCollection();
              
            //_serialPortsObservable = new SerialPortWatcher().ComPorts.ToObservable();
            //_serialPortsObservable
            //    .Subscribe(CommPort.Add);

            _selectedCommPort = CommPort.Contains(SettingsManager.SettingsInstance.InstrumentCommPort) ? SettingsManager.SettingsInstance.InstrumentCommPort : string.Empty;
            _selectedBaudRate = BaudRate.Contains(SettingsManager.SettingsInstance.InstrumentBaudRate) ? SettingsManager.SettingsInstance.InstrumentBaudRate : -1;
            _selectedTachCommPort = TachCommPort.Contains(SettingsManager.SettingsInstance.TachCommPort) ? SettingsManager.SettingsInstance.TachCommPort : string.Empty;

            this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort)
                .Subscribe(_ =>
                {
                    SettingsManager.SettingsInstance.InstrumentBaudRate = _.Item1;
                    SettingsManager.SettingsInstance.InstrumentCommPort = _.Item2;
                    SettingsManager.SettingsInstance.TachCommPort = _.Item3;
                    SettingsManager.Save();
                });

            /*** Commands ***/
            var canStartNewTest = this.WhenAnyValue(x => x.SelectedInstrument, x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort,
                (instrumentType, baud, instrumentPort, tachPort) => 
                    instrumentType != null && BaudRate.Contains(baud) && !string.IsNullOrEmpty(instrumentPort) && !string.IsNullOrEmpty(tachPort));

            StartTestCommand = ReactiveCommand.Create(canStartNewTest);
            StartTestCommand
                .Subscribe(async _ => await StartNewQaTest());

            _viewContext = NewQaTestViewContext;
        }

        private IObservable<string> PortsWatcherObservable()
        {
            return Observable.Create<string>(observer =>
            {
                return Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    .Subscribe(
                        _ =>
                        {
                            if (!_commPort.SequenceEqual(SerialPort.GetPortNames()))
                            {
                                SerialPort.GetPortNames().ForEach(observer.OnNext);
                            }
                        });
            });
        }

        private string _viewContext;
        public string ViewContext
        {
            get { return _viewContext; }
            set { this.RaiseAndSetIfChanged(ref _viewContext, value); }
        }

        public InstrumentType SelectedInstrument => InstrumentTypes?.FirstOrDefault(i => i.IsSelected)?.Instrument;
        private ReactiveList<SelectableInstrumentType> _instrumentTypes;
        public ReactiveList<SelectableInstrumentType> InstrumentTypes
        {
            get { return _instrumentTypes; }
            set { this.RaiseAndSetIfChanged(ref _instrumentTypes, value); }
        }

        private IReactiveDerivedList<string> _commPort;
        public IReactiveDerivedList<string> CommPort
        {
            get { return _commPort; }
            set { this.RaiseAndSetIfChanged(ref _commPort, value); }
        }

        private string _selectedCommPort;
        public string SelectedCommPort
        {
            get { return _selectedCommPort; }
            set { this.RaiseAndSetIfChanged(ref _selectedCommPort, value); }
        }

        public List<string> TachCommPort => SerialPort.GetPortNames().ToList();
        private string _selectedTachCommPort;
        public string SelectedTachCommPort
        {
            get { return _selectedTachCommPort; }
            set { this.RaiseAndSetIfChanged(ref _selectedTachCommPort, value); }
        }

        public List<int> BaudRate => CommProtocol.Common.IO.SerialPort.BaudRates;
        private int _selectedBaudRate;
        public int SelectedBaudRate
        {
            get { return _selectedBaudRate; }
            set { this.RaiseAndSetIfChanged(ref _selectedBaudRate, value); }
        }
        
        public async Task CancelCommand()
        {
            await ScreenManager.GoHome();
        }

        public async Task StartNewQaTest()
        {
            if (SelectedInstrument != null)
            {
                SettingsManager.SettingsInstance.LastInstrumentTypeUsed = SelectedInstrument.Name;
                SettingsManager.Save();

                try
                {
                    var qaTestRun = ScreenManager.ResolveViewModel<QaTestRunInteractiveViewModel>();
                    await qaTestRun.Initialize(SelectedInstrument);
                    await ScreenManager.ChangeScreen(qaTestRun);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                }
            }
        }

        //private void VerifySettings()
        //{
        //    _selectedCommPort = SettingsManager.SettingsInstance.InstrumentCommPort;
        //    _selectedBaudRate = SettingsManager.SettingsInstance.InstrumentBaudRate;
        //    _selectedTachCommPort = SettingsManager.SettingsInstance.TachCommPort;

        //    NotifyOfPropertyChange(() => SelectedCommPort);
        //    NotifyOfPropertyChange(() => SelectedBaudRate);
        //    NotifyOfPropertyChange(() => SelectedTachCommPort);

        //    if (string.IsNullOrEmpty(SelectedCommPort))
        //        ScreenManager.ShowWindow(new SettingsViewModel(ScreenManager, EventAggregator));
        //}

        public class SelectableInstrumentType
        {
            public InstrumentType Instrument { get; set; }
            public bool IsSelected { get; set; }
        }
    }
}