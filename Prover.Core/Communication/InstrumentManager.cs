using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MccDaq;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
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
        private TachometerCommunication _tachCommunication;
        private bool _keepLiveReading = false;

        public DataAcqBoard OutputBoard { get; private set; }
        public DataAcqBoard AInputBoard { get; private set; }
        public DataAcqBoard BInputBoard { get; private set; }
        
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
            CommPort = Communications.CreateCommPortObject(commName, baudRate);
            _instrumentCommunication = new InstrumentCommunication(CommPort, _instrument);
        }

        public void SetupTachCommPort(string commName)
        {
            if (_tachCommunication == null)
                _tachCommunication = new TachometerCommunication(commName);

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
            _instrument.Volume.LoadMeterIndex();
        }

        public async Task DownloadVolumeAfterTestItems()
        {
            _instrument.Volume.TestInstrumentValues = await _instrumentCommunication.DownloadItemsAsync(_instrument.Volume.AfterTestItems);
        }

        public async Task StartLiveReadTemperature()
        {
            await _instrumentCommunication.Connect();

            do
            {
                var liveValue = await _instrumentCommunication.LiveReadItem(26);
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new LiveReadEvent(liveValue));
            } while (_keepLiveReading);
        }

        public async Task StopLiveReadTemperature()
        {
            if (_keepLiveReading)
            {
                _keepLiveReading = false;
                await _instrumentCommunication.Disconnect();
            } 
        }

        public void Save()
        {
            var store = new InstrumentStore();
            store.Upsert(_instrument);
        }

        public async Task StartVolumeTest()
        {
            await Task.Run(async () =>
            {
                await _instrumentCommunication.Disconnect();

                OutputBoard = new DataAcqBoard(0, 0, 0);
                AInputBoard = new DataAcqBoard(0, DigitalPortType.FirstPortA, 0);
                BInputBoard = new DataAcqBoard(0, DigitalPortType.FirstPortB, 1);

                //Reset Tach setting
                if (_tachCommunication != null) await _tachCommunication.ResetTach();

                Instrument.Volume.PulseACount = 0;
                Instrument.Volume.PulseBCount = 0;

                OutputBoard.StartMotor();

                System.Threading.Thread.Sleep(500);
            });
        }


        public async Task StopVolumeTest()
        {
            await Task.Run(async () =>
            {
                OutputBoard.StopMotor();
                System.Threading.Thread.Sleep(2000);

                if (_tachCommunication != null)
                {
                    Instrument.Volume.AppliedInput = await _tachCommunication.ReadTach();
                }

                await DownloadVolumeAfterTestItems();
                await _instrumentCommunication.Disconnect();
            });
        }
    }
}
