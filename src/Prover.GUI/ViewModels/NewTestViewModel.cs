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
using Settings = Prover.Core.Settings;

namespace Prover.GUI.ViewModels
{
    public class NewTestViewModel : ReactiveScreen
    {
        readonly IUnityContainer _container;

        

        public NewTestViewModel(IUnityContainer container)
        {
            _container = container;
            CommPortName = Settings.Instrument.CommPortName;
            BaudRate = Settings.Instrument.BaudRate;
            TachCommName = Settings.Tachometer.CommPortName;
        }

        public InstrumentManager InstrumentManager { get; set; }
        
        public ICommPort CommPort { get; set; }
        public string CommPortName { get; set; }
        public string TachCommName { get; set; }
        public BaudRateEnum BaudRate { get; set; }
        public Instrument Instrument => InstrumentManager.Instrument;

        //public List<String> BaudRates
        //{
        //    get { return Enum.GetNames(typeof(BaudRateEnum)).ToList(); }
        //}

        //public List<string> CommPorts
        //{
        //    get { return Communications.GetCommPortList(); }
        //}

        //public List<string> TachPorts
        //{
        //    get { return Communications.GetCommPortList().Where(c => !c.Contains("IrDA")).ToList(); }
        //}

        #region Methods

        //public string SelectedCommPort()
        //{
        //    return Settings.Default.CommPort;
        //}

        //public string SelectedTachCommPort()
        //{
        //    return Settings.Default.TachCommPort;
        //}

        //public string SelectedBaudRate()
        //{
        //    return Settings.Default.BaudRate;
        //}

        //public void SetCommPort(string comm)
        //{
        //    CommPort = comm;
        //    Settings.Default.CommPort = comm;
        //}

        //public void SetTachCommPort(string comm)
        //{
        //    TachCommName = comm;
        //    Settings.Default.TachCommPort = comm;
        //}

        //public void SetBaudRate(string baudRate)
        //{
        //    BaudRate = (BaudRateEnum) Enum.Parse(typeof (BaudRateEnum), baudRate);
        //    Settings.Default.BaudRate = baudRate;
        //}

        //public void RefreshCommSettingsCommand()
        //{
        //    NotifyOfPropertyChange(() => CommPorts);
        //    NotifyOfPropertyChange(() => TachPorts);
        //}

        public async void FetchInstrumentItems()
        {
            await Task.Run((Func<Task>)(async () =>
            {
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting download from instrument..."));
                if (this.CommPortName == null)
                {
                    MessageBox.Show("Please select a Comm Port and Baud Rate first.", "Comm Port");
                    return;
                }
                if (InstrumentManager == null)
                {
                    InstrumentManager = new InstrumentManager(_container, CommPortName, BaudRate);
                    if (TachCommName != null) InstrumentManager.SetupTachCommPort(TachCommName);
                }

                await InstrumentManager.DownloadInfo();

                base.NotifyOfPropertyChange(() => Instrument);
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Completed Download from Instrument!"));
                //Publish the change in instrument state to anyone who's listening
                _container.Resolve<IEventAggregator>().PublishOnUIThread(new InstrumentUpdateEvent(InstrumentManager));
            }));
           
        }

        public async void SaveInstrument()
        {
            if (InstrumentManager == null) return;

            if (!Instrument.HasPassed && MessageBox.Show("This instrument hasn't passed all tests." + Environment.NewLine + "Would you still like to save?", "Save", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            await InstrumentManager.SaveAsync();
            _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Successfully Saved instrument!"));
        }
        #endregion

        #region Views
        public SiteInformationViewModel SiteInformationItem => new SiteInformationViewModel(_container);
        public TemperatureViewModel TemperatureInformationItem => new TemperatureViewModel(_container);
        public VolumeViewModel VolumeInformationItem => new VolumeViewModel(_container);
        #endregion
    }
}
