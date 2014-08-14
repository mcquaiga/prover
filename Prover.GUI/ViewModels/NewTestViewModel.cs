using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.SerialProtocol;
using ReactiveUI;

namespace Prover.GUI.ViewModels
{
    public class NewTestViewModel : ReactiveScreen
    {
        public InstrumentManager InstrumentManager { get; set; }

        public NewTestViewModel()
        {
            InstrumentManager = new InstrumentManager();
        }

        private ObservableAsPropertyHelper<Instrument> _Instrument;
        Instrument _instrument;
        public Instrument Instrument
        {
            get { return _instrument; }
            set { this.RaiseAndSetIfChanged(ref _instrument, value);  }
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

        public void FetchInstrumentItems()
        {
            if (CommName == null) MessageBox.Show("Please select a Comm Port and Baud Rate first.", "Comm Port");

            CommPort = InstrumentCommunication.CreateCommPortObject(CommName, BaudRate);

            InstrumentManager.CommPort = CommPort;
            InstrumentManager.DownloadInstrumentItems();
            Instrument = InstrumentManager.Instrument;
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
