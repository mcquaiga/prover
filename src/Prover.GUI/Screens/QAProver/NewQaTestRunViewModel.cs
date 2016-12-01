using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Akka.Util.Internal;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.DriveTypes;
using Prover.Core.Events;
using Prover.Core.Settings;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver.VerificationTestViews;
using Prover.GUI.Screens.Settings;
using ReactiveUI;

namespace Prover.GUI.Screens.QAProver
{
    public class NewQaTestRunViewModel : ViewModelBase, IHandle<SettingsChangeEvent>
    {
        private const string RotaryDriveName = "Rotary";
        private const string MechanicalDriveName = "Mechanical";

        private ReactiveList<SelectableInstrumentType> _instrumentTypes = new ReactiveList<SelectableInstrumentType>();
        private int _selectedBaudRate;
        private string _selectedCommPort;
        private string _selectedTachCommPort;

        public NewQaTestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            /*
             Setup Instruments list
             */
            Instruments.GetAll().ForEach(
                x => InstrumentTypes.Add(new SelectableInstrumentType {Instrument = x, IsSelected = false}));

            SetDefaultValues();

            this.WhenAnyValue(x => x.SelectedBaudRate, x => x.SelectedCommPort, x => x.SelectedTachCommPort)
                .Subscribe(_ =>
                {
                    SettingsManager.SettingsInstance.InstrumentBaudRate = _.Item1;
                    SettingsManager.SettingsInstance.InstrumentCommPort = _.Item2;
                    SettingsManager.SettingsInstance.TachCommPort = _.Item3;
                    SettingsManager.Save();
                });

        }

        private void SetDefaultValues()
        {
            SettingsManager.RefreshSettings();

            _selectedCommPort = SettingsManager.SettingsInstance.InstrumentCommPort;
            _selectedBaudRate = SettingsManager.SettingsInstance.InstrumentBaudRate;
            _selectedTachCommPort = SettingsManager.SettingsInstance.TachCommPort;

            var lastSelected = InstrumentTypes.FirstOrDefault(
                i => i.Instrument.Name == SettingsManager.SettingsInstance.LastInstrumentTypeUsed);
            if (lastSelected != null) lastSelected.IsSelected = true;

            //Drive types
            var driveType = SettingsManager.SettingsInstance.LastDriveTypeUsed;
            if (driveType == RotaryDriveName)
                _rotaryDriveChecked = true;
            else
                _mechanicalDriveChecked = true;
        }

        public ReactiveList<SelectableInstrumentType> InstrumentTypes
        {
            get { return _instrumentTypes; }
            set { this.RaiseAndSetIfChanged(ref _instrumentTypes, value); }
        }

        public InstrumentType SelectedInstrument => InstrumentTypes.FirstOrDefault(i => i.IsSelected)?.Instrument;

        public List<string> CommPort => System.IO.Ports.SerialPort.GetPortNames().ToList();
        public string SelectedCommPort
        {
            get { return _selectedCommPort; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCommPort, value);
            }
        }

        public List<string> TachCommPort => System.IO.Ports.SerialPort.GetPortNames().ToList();
        public string SelectedTachCommPort
        {
            get { return _selectedTachCommPort; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedTachCommPort, value);
            }
        }

        public List<int> BaudRate => SerialPort.BaudRates;
        public int SelectedBaudRate
        {
            get { return _selectedBaudRate; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedBaudRate, value);
            }
        }

        private bool _rotaryDriveChecked;
        public bool RotaryDriveChecked
        {
            get { return _rotaryDriveChecked; }
            set
            {
                if (value.Equals(_rotaryDriveChecked)) return;
                _rotaryDriveChecked = value;
                if (_rotaryDriveChecked)
                {
                    SettingsManager.SettingsInstance.LastDriveTypeUsed = RotaryDriveName;
                    SettingsManager.Save();
                }

                NotifyOfPropertyChange(() => RotaryDriveChecked);
            }
        }

        private bool _mechanicalDriveChecked;
        public bool MechanicalDriveChecked
        {
            get { return _mechanicalDriveChecked; }
            set
            {
                if (value.Equals(_mechanicalDriveChecked)) return;
                _mechanicalDriveChecked = value;

                if (_mechanicalDriveChecked)
                {
                    SettingsManager.SettingsInstance.LastDriveTypeUsed = MechanicalDriveName;
                    SettingsManager.Save();
                }

                NotifyOfPropertyChange(() => MechanicalDriveChecked);
            }
        }

        public void Handle(SettingsChangeEvent message)
        {
            VerifySettings();
        }
 
        public async Task CancelCommand()
        {
            await ScreenManager.GoHome();
        }

        public async Task InitializeTest()
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

        private void VerifySettings()
        {
            _selectedCommPort = SettingsManager.SettingsInstance.InstrumentCommPort;
            _selectedBaudRate = SettingsManager.SettingsInstance.InstrumentBaudRate;
            _selectedTachCommPort = SettingsManager.SettingsInstance.TachCommPort;

            NotifyOfPropertyChange(() => SelectedCommPort);
            NotifyOfPropertyChange(() => SelectedBaudRate);
            NotifyOfPropertyChange(() => SelectedTachCommPort);

            if (string.IsNullOrEmpty(SelectedCommPort))
                ScreenManager.ShowWindow(new SettingsViewModel(ScreenManager, EventAggregator));
        }

        public class SelectableInstrumentType
        {
            public InstrumentType Instrument { get; set; }
            public bool IsSelected { get; set; }
        }
    }
}