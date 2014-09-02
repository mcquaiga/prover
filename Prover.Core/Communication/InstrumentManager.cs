using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.SerialProtocol;

namespace Prover.Core.Communication
{
    public class InstrumentManager
    {
        private readonly Instrument _instrument;
        private readonly IUnityContainer _container;
        public ICommPort CommPort { get; set; }

        public Instrument Instrument
        {
            get { return _instrument; }
        }

        public InstrumentManager(IUnityContainer container)
        {
            _container = container;
            _instrument = new Instrument();
            _instrument.Temperature = new Temperature(_instrument);
            _instrument.Volume = new Volume(_instrument);
        }

        public InstrumentManager(IUnityContainer container, string commName, BaudRateEnum baudRate) : this(container)
        {
            SetupCommPort(commName, baudRate);
        }

        public void SetupCommPort(string commName, BaudRateEnum baudRate)
        {
            CommPort = InstrumentCommunication.CreateCommPortObject(commName, baudRate);
        }

        public InstrumentManager(IUnityContainer container, ICommPort commPort)
        {
            _instrument = new Instrument();
            CommPort = commPort;
        }

        public InstrumentManager(Instrument instrument, ICommPort commPort)
        {
            _instrument = instrument;
            CommPort = commPort;
        }

        public async Task DownloadInstrumentItemsAsync()
        {
            _instrument.InstrumentValues = await InstrumentCommunication.DownloadItemsAsync(CommPort, _instrument, _instrument.Items);
        }

        public async Task DownloadTemperatureItems()
        {
            _instrument.Temperature.InstrumentValues = await InstrumentCommunication.DownloadItemsAsync(CommPort, _instrument,_instrument.Temperature.Items);
        }

        public async Task DownloadTemperatureTestItems(TemperatureTest.Level level)
        {
            var test = _instrument.Temperature.Tests.FirstOrDefault(x => x.TestLevel == level);
            if (test != null)
                test.InstrumentValues = await InstrumentCommunication.DownloadItemsAsync(CommPort, _instrument, test.Items);
        }

        public void Save()
        {
            var store = new InstrumentStore();
            store.Save(_instrument);
        }
    }
}
