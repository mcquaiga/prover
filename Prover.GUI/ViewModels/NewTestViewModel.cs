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
using Prover.GUI.ViewModels.TemperatureViews;
using Prover.GUI.Views;
using Prover.GUI.Views.TemperatureViews;
using Prover.SerialProtocol;
using ReactiveUI;

namespace Prover.GUI.ViewModels
{
    public class NewTestViewModel : ReactiveScreen
    {
        private readonly IUnityContainer _container;
        private string TachCommName;
        public InstrumentManager InstrumentManager { get; set; }

        public NewTestViewModel(IUnityContainer container)
        {
            _container = container;
            
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
            get { return Communications.GetCommPortList(); }
        }

        public List<string> TachPorts
        {
            get { return Communications.GetCommPortList().Where(c => !c.Contains("IrDA")).ToList(); }
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

        public string SelectedCommPort()
        {
            return Settings.Default.CommPort;
        }

        public string SelectedTachCommPort()
        {
            return Settings.Default.TachCommPort;
        }

        public string SelectedBaudRate()
        {
            return Settings.Default.BaudRate;
        }

        public void SetCommPort(string comm)
        {
            CommName = comm;
            Settings.Default.CommPort = comm;
        }

        public void SetTachCommPort(string comm)
        {
            TachCommName = comm;
            Settings.Default.TachCommPort = comm;
            if (InstrumentManager != null) InstrumentManager.SetupTachCommPort(comm);
        }

        public void SetBaudRate(string baudRate)
        {
            BaudRate = (BaudRateEnum) Enum.Parse(typeof (BaudRateEnum), baudRate);
            Settings.Default.BaudRate = baudRate;
        }

        public void RefreshCommSettingsCommand()
        {
            NotifyOfPropertyChange(() => CommPorts);
            NotifyOfPropertyChange(() => TachPorts);
        }

        public async void FetchInstrumentItems()
        {
            await Task.Run(async () =>
            {
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting download from instrument..."));
                if (CommName == null)
                {
                    MessageBox.Show("Please select a Comm Port and Baud Rate first.", "Comm Port");
                    return;
                }
                if (InstrumentManager == null)
                {
                    InstrumentManager = new InstrumentManager(_container);
                    InstrumentManager.SetupCommPort(CommName, BaudRate);
                }

                await InstrumentManager.DownloadInfo();

                NotifyOfPropertyChange(() => Instrument);
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Completed Download from Instrument!"));
                //Publish the change in instrument state to anyone who's listening
                _container.Resolve<IEventAggregator>().PublishOnUIThread(new InstrumentUpdateEvent(InstrumentManager));
            });
           
        }

        public async void SaveInstrument()
        {
            if (InstrumentManager == null) return;

            await InstrumentManager.SaveAsync();
            _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Successfully Saved instrument!"));
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

        public VolumeViewModel VolumeInformationItem
        {
            get { return new VolumeViewModel(_container); }
        }
        #endregion
    }
}
