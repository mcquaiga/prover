using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
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
        private InstrumentCommunication _instrumentCommunication;

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
            _instrumentCommunication = new InstrumentCommunication(CommPort, _instrument);
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

        public async Task DownloadInfo()
        {
            await _instrumentCommunication.Connect();
            await DownloadInstrumentItemsAsync();
            await DownloadTemperatureItems();
            await DownloadVolumeItems();
            await _instrumentCommunication.Disconnect();
        }

        public async Task DownloadInstrumentItemsAsync()
        {
            _instrument.InstrumentValues = await _instrumentCommunication.DownloadItemsAsync( _instrument.Items);
        }

        public async Task DownloadTemperatureItems()
        {
            _instrument.Temperature.InstrumentValues = await _instrumentCommunication.DownloadItemsAsync(_instrument.Temperature.Items);
        }

        public async Task DownloadTemperatureTestItems(TemperatureTest.Level level)
        {
            await _instrumentCommunication.Connect();
            var test = _instrument.Temperature.Tests.FirstOrDefault(x => x.TestLevel == level);
            if (test != null)
                test.InstrumentValues = await _instrumentCommunication.DownloadItemsAsync(test.Items);
            await _instrumentCommunication.Disconnect();
        }

        public async Task DownloadVolumeItems()
        {
            _instrument.Volume.InstrumentValues = await _instrumentCommunication.DownloadItemsAsync(_instrument.Volume.Items);
        }

        public async Task DownloadVolumeAfterTestItems()
        {
            _instrument.Volume.TestInstrumentValues = await _instrumentCommunication.DownloadItemsAsync(_instrument.Volume.AfterTestItems);
        }

        public void Save()
        {
            var store = new InstrumentStore();
            store.Upsert(_instrument);
        }

        
    }
}
