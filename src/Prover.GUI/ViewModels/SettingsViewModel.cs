using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prover.SerialProtocol;
using Prover.Core.Communication;
using Prover.Core.Settings;
using Caliburn.Micro.ReactiveUI;

namespace Prover.GUI.ViewModels
{
    public class SettingsViewModel : ReactiveScreen
    {
        private IUnityContainer _container;

        public SettingsViewModel(IUnityContainer _container)
        {
            this._container = _container;
        }

        public List<String> BaudRates
        {
            get { return Enum.GetNames(typeof(BaudRateEnum)).ToList(); }
        }

        public List<string> CommPorts
        {
            get { return Communications.GetCommPortList(); }
        }

        public List<string> TachPorts
        {
            get { return Communications.GetCommPortList().Where(c => !c.Contains("IrDA")).ToList(); }
        }

        public static string SelectedCommPort()
        {
            return Instrument.CommPortName;
        }

        public static string SelectedTachCommPort()
        {
            return Tachometer.CommPortName;
        }

        public static string SelectedBaudRate()
        {
            return Instrument.BaudRate.ToString();
        }

        public static void SetCommPort(string comm)
        {
            Instrument.CommPortName = comm;
        }

        public static void SetTachCommPort(string comm)
        {
            Tachometer.CommPortName = comm;
        }

        public static void SetBaudRate(string baudRate)
        {
            Instrument.BaudRate = (BaudRateEnum)Enum.Parse(typeof(BaudRateEnum), baudRate);
        }

        public void RefreshCommSettingsCommand()
        {
            NotifyOfPropertyChange(() => CommPorts);
            NotifyOfPropertyChange(() => TachPorts);
        }
    }
}
