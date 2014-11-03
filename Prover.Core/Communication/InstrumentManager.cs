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
        private bool _isLiveReading = false;
        private bool _stopLiveReading;
        private bool _isBusy = false;
        

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
            if (!_isBusy)
            {
                await _instrumentCommunication.Connect();
                await DownloadInstrumentItemsAsync();
                await DownloadTemperatureItems();
                await DownloadVolumeItems();
                await _instrumentCommunication.Disconnect();
            }
        }

        public async Task DownloadInstrumentItemsAsync()
        {
            if (!_isBusy)
            {
                _isBusy = true;
                _instrument.InstrumentValues = await _instrumentCommunication.DownloadItemsAsync(_instrument.Items);
                _isBusy = false;
            }
        }

        public async Task DownloadTemperatureItems()
        {
            if (!_isBusy)
            {
                _isBusy = true;
                _instrument.Temperature.InstrumentValues = await _instrumentCommunication.DownloadItemsAsync(_instrument.Temperature.Items);
                _isBusy = false;
            }
            
        }

        public async Task DownloadTemperatureTestItems(TemperatureTest.Level level)
        {
            if (!_isBusy || (_isBusy && _isLiveReading))
            {
                if (_isLiveReading) StopLiveReadTemperature();
                System.Threading.Thread.Sleep(1000);
                _isBusy = true;
                await _instrumentCommunication.Connect();
                var test = _instrument.Temperature.Tests.FirstOrDefault(x => x.TestLevel == level);
                if (test != null)
                    test.InstrumentValues = await _instrumentCommunication.DownloadItemsAsync(test.Items);
                await _instrumentCommunication.Disconnect();
                _isBusy = false;
            }
        }

        public async Task DownloadVolumeItems()
        {
            if (!_isBusy)
            {
                _isBusy = true;
                _instrument.Volume.InstrumentValues = await _instrumentCommunication.DownloadItemsAsync(_instrument.Volume.Items);
                _instrument.Volume.LoadMeterIndex();
                _isBusy = false;
            }
           
        }

        public async Task DownloadVolumeAfterTestItems()
        {
            if (!_isBusy)
            {
                _isBusy = true;
                _instrument.Volume.TestInstrumentValues = await _instrumentCommunication.DownloadItemsAsync(_instrument.Volume.AfterTestItems);
                _isBusy = false;
            }
            
        }

        public async Task StartLiveReadTemperature()
        {
            if (!_isBusy)
            {
                await _instrumentCommunication.Connect();
                _stopLiveReading = false;
                _isLiveReading = true;
                do
                {
                    var liveValue = await _instrumentCommunication.LiveReadItem(26);
                    _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new LiveReadEvent(liveValue));
                } while (!_stopLiveReading);

                await _instrumentCommunication.Disconnect();
                _isLiveReading = false;
                _isBusy = false;
            } 
        }

        public void StopLiveReadTemperature()
        {
            if (_isLiveReading)
            {
                _stopLiveReading = true;
            }
        }

        public async Task SaveAsync()
        {
            var store = new InstrumentStore(_container);
            await store.UpsertAsync(_instrument);
        }

        public async Task StartVolumeTest()
        {
            if (!_isBusy)
            {
                await Task.Run(async () =>
                {
                    _isBusy = true;
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
                    _isBusy = false;
                });
            }
        }


        public async Task StopVolumeTest()
        {
            if (!_isBusy)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        _isBusy = true;
                        OutputBoard.StopMotor();

                        if (_tachCommunication != null)
                        {
                            Instrument.Volume.AppliedInput = await _tachCommunication.ReadTach();
                            _tachCommunication.Dispose();
                        }
                       
                        await DownloadVolumeAfterTestItems();
                        await _instrumentCommunication.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        _isBusy = false;
                    }
                });
            } 
        }
    }
}
