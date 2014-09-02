using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using Prover.GUI.Properties;
using Prover.SerialProtocol;
using ReactiveUI;

namespace Prover.GUI.ViewModels
{
    public class NewTestViewModel : ReactiveScreen
    {
        private readonly IUnityContainer _container;
        public InstrumentManager InstrumentManager { get; set; }

        public NewTestViewModel(IUnityContainer container)
        {
            _container = container;
            InstrumentManager = new InstrumentManager(_container);
        }

        public Instrument Instrument
        {
            get { return InstrumentManager.Instrument; }
        }

        public ICommPort CommPort { get; set; }
        public string CommName { get; set; }
        public BaudRateEnum BaudRate { get; set; }

        public List<String> BaudRates
        {
            get { return Enum.GetNames(typeof(BaudRateEnum)).ToList(); }
        }

        public List<string> CommPorts
        {
            get { return InstrumentCommunication.GetCommPortList(); }
        }

        public bool CommPortSettings(string commName)
        {
            return (Settings.Default.CommPort == commName);
        }

        public bool BaudRateSettings(string baudRate)
        {
            return (Settings.Default.BaudRate == baudRate);
        }

        #region Methods
        public void SetCommPort(string comm)
        {
            CommName = comm;
            Settings.Default.CommPort = comm;
        }

        public void SetBaudRate(string baudRate)
        {
            BaudRate = (BaudRateEnum) Enum.Parse(typeof (BaudRateEnum), baudRate);
            Settings.Default.BaudRate = baudRate;
        }

        public async void FetchInstrumentItems()
        {
            _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting download from instrument..."));
            _container.Resolve<IEventAggregator>().PublishOnUIThread(new InstrumentUpdateEvent(InstrumentManager));
            if (CommName == null)
            {
                MessageBox.Show("Please select a Comm Port and Baud Rate first.", "Comm Port");
                return;
            }
            InstrumentManager.SetupCommPort(CommName, BaudRate);

            await InstrumentManager.DownloadInstrumentItemsAsync();
            NotifyOfPropertyChange(() => Instrument);

            //Publish the change in instrument state to anyone who's listening
            
        }

        public void SaveInstrument()
        {
            if (InstrumentManager != null)
            {
                InstrumentManager.Save();
            }      
        }
        #endregion

        #region Views
        public SiteInformationViewModel SiteInformationItem
        {
            get { return new SiteInformationViewModel(_container); }
        }

        public TemperatureViewModel TemperatureInformationItem
        {
            get {  return new TemperatureViewModel(_container);}
        }
        #endregion
    }
}
