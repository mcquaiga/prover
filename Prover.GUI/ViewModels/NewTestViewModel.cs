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
using Prover.SerialProtocol;
using ReactiveUI;

namespace Prover.GUI.ViewModels
{
    public class NewTestViewModel : ReactiveScreen
    {
        private IUnityContainer _container;
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

        //Views
        public SiteInformationViewModel SiteInformationItem
        {
            get { return new SiteInformationViewModel(_container); }
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

        //Actions
        public void SetCommPort(string comm)
        {
            CommName = comm;
        }

        public void SetBaudRate(string baudRate)
        {
            BaudRate = (BaudRateEnum) Enum.Parse(typeof (BaudRateEnum), baudRate);
        }

        public async void FetchInstrumentItems()
        {
            //Publish the change in instrument state to anyone who's listening
            _container.Resolve<IEventAggregator>().Publish(Instrument, action => Task.Factory.StartNew(action));
            if (CommName == null)
            {
                MessageBox.Show("Please select a Comm Port and Baud Rate first.", "Comm Port");
                return;
            }

            CommPort = InstrumentCommunication.CreateCommPortObject(CommName, BaudRate);

            InstrumentManager.CommPort = CommPort;
            await InstrumentManager.DownloadInstrumentItemsAsync();
            NotifyOfPropertyChange(() => Instrument);

            
        }

        public void SaveInstrument()
        {
            if (InstrumentManager != null)
            {
                InstrumentManager.Save();
            }      
        }
    }
}
