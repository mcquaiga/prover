using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.Core.Settings;
using Prover.SerialProtocol;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Windows;

namespace Prover.GUI.ViewModels
{
    public class SettingsViewModel : ReactiveScreen, IWindowSettings
    {
        IUnityContainer _container;
        string          _selectedCommPort;
        BaudRateEnum    _selectedBaudRate;
        string          _selectedTachCommPort;

        public dynamic WindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.MinWidth = 450;
                settings.Title = @"Settings";
                return settings;
            }
        }
        
        public SettingsViewModel(IUnityContainer container)
        {
            _container = container;
            _selectedBaudRate = SettingsManager.SettingsInstance.InstrumentBaudRate;
            _selectedCommPort = SettingsManager.SettingsInstance.InstrumentCommPort;
            _selectedTachCommPort = SettingsManager.SettingsInstance.TachCommPort;
        }

        public List<BaudRateEnum> BaudRate => Enum.GetValues(typeof(BaudRateEnum)).Cast<BaudRateEnum>().ToList();
        public List<string> CommPort => Communications.GetCommPortList();
        public List<string> TachCommPort => Communications.GetCommPortList().Where(c => !c.Contains("IrDA")).ToList();

    

        public string SelectedCommPort
        {
            get
            {
                return _selectedCommPort;
            }
            set
            {
                _selectedCommPort = value;
                SettingsManager.SettingsInstance.InstrumentCommPort = value;
                SettingsManager.Save();
            }
        }

        public string SelectedTachCommPort
        {
            get
            {
                return _selectedTachCommPort;
            }
            set
            {
                SettingsManager.SettingsInstance.TachCommPort = value;
                _selectedTachCommPort = value;
                SettingsManager.Save();
            }
        }

        public BaudRateEnum SelectedBaudRate
        {
            get
            {
                return _selectedBaudRate;
            }
            set
            {
                SettingsManager.SettingsInstance.InstrumentBaudRate = value;
                SettingsManager.Save();
                _selectedBaudRate = value;
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            _container.Resolve<IEventAggregator>().PublishOnUIThreadAsync(new SettingsChangeEvent());
            base.CanClose(callback);
        }

        //public static void SetCommPort(string comm)
        //{
        //    Instrument.CommPortName = comm;
        //}

        //public static void SetTachCommPort(string comm)
        //{
        //    Tachometer.CommPortName = comm;
        //}

        //public static void SetBaudRate(string baudRate)
        //{
        //    Instrument.BaudRate = (BaudRateEnum)Enum.Parse(typeof(BaudRateEnum), baudRate);
        //}

        public void RefreshCommSettingsCommand()
        {
            NotifyOfPropertyChange(() => CommPort);
            NotifyOfPropertyChange(() => TachCommPort);
        }
    }
}