using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common.IO;
using Prover.Core.Events;
using Prover.Core.Settings;
using Prover.GUI.Common;

namespace Prover.GUI.Screens.Settings
{
    public class SettingsViewModel : ReactiveScreen, IWindowSettings
    {
        private readonly IUnityContainer _container;
        private int _selectedBaudRate;
        private string _selectedCommPort;
        private string _selectedTachCommPort;

        public SettingsViewModel(IUnityContainer container)
        {
            _container = container;
            _selectedBaudRate = SettingsManager.SettingsInstance.InstrumentBaudRate;
            _selectedCommPort = SettingsManager.SettingsInstance.InstrumentCommPort;
            _selectedTachCommPort = SettingsManager.SettingsInstance.TachCommPort;
        }

        public List<int> BaudRate => SerialPort.BaudRates;
        public List<string> CommPort => System.IO.Ports.SerialPort.GetPortNames().ToList();
        public List<string> TachCommPort => System.IO.Ports.SerialPort.GetPortNames().ToList();

        public string SelectedCommPort
        {
            get { return _selectedCommPort; }
            set
            {
                _selectedCommPort = value;
                SettingsManager.SettingsInstance.InstrumentCommPort = value;
                SettingsManager.Save();
            }
        }

        public string SelectedTachCommPort
        {
            get { return _selectedTachCommPort; }
            set
            {
                SettingsManager.SettingsInstance.TachCommPort = value;
                _selectedTachCommPort = value;
                SettingsManager.Save();
            }
        }

        public int SelectedBaudRate
        {
            get { return _selectedBaudRate; }
            set
            {
                SettingsManager.SettingsInstance.InstrumentBaudRate = value;
                SettingsManager.Save();
                _selectedBaudRate = value;
            }
        }

        public dynamic WindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.MinWidth = 450;
                settings.Title = "Settings";
                return settings;
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            _container.Resolve<IEventAggregator>().PublishOnUIThreadAsync(new SettingsChangeEvent());
            base.CanClose(callback);
        }

        public void RefreshCommSettingsCommand()
        {
            NotifyOfPropertyChange(() => CommPort);
            NotifyOfPropertyChange(() => TachCommPort);
        }
    }
}