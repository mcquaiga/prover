using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Settings;
using Prover.SerialProtocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prover.GUI.ViewModels
{
    public class SettingsViewModel : ReactiveScreen
    {
        IUnityContainer _container;
        string          _selectedCommPort;
        BaudRateEnum    _selectedBaudRate;
        string          _selectedTachCommPort;

        public SettingsViewModel(IUnityContainer _container)
        {
            this._container = _container;
            _selectedBaudRate = SettingsManager.SettingsInstance.InstrumentBaudRate;
            _selectedCommPort = SettingsManager.SettingsInstance.InstrumentCommPort;
            _selectedTachCommPort = SettingsManager.SettingsInstance.TachCommPort;
        }

        public List<String> BaudRate
        {
            get { return Enum.GetNames(typeof(BaudRateEnum)).ToList(); }
        }

        public List<string> CommPort
        {
            get { return Communications.GetCommPortList(); }
        }

        public List<string> TachCommPort
        {
            get { return Communications.GetCommPortList().Where(c => !c.Contains("IrDA")).ToList(); }
        }

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