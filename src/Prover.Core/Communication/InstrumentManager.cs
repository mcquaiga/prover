using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MccDaq;
using Microsoft.Practices.Unity;
using NLog;
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
        private bool _isLiveReading = false;
        private bool _stopLiveReading;
        private bool _isBusy = false;
        private string _tachCommPort;
        private Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private bool _runningTest;

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
            _runningTest = false;

            OutputBoard = new DataAcqBoard(0, 0, 0);
            AInputBoard = new DataAcqBoard(0, DigitalPortType.FirstPortA, 0);
            BInputBoard = new DataAcqBoard(0, DigitalPortType.FirstPortB, 1);
        }

        public InstrumentManager(IUnityContainer container, string commName, BaudRateEnum baudRate, string tachCommName) : this(container)
        {
            SetupCommPort(commName, baudRate);
            _tachCommPort = tachCommName;
        }

        public void SetupCommPort(string commName, BaudRateEnum baudRate)
        {
            CommPort = Communications.CreateCommPortObject(commName, baudRate);
            if (CommPort == null) throw new NullReferenceException("No comm port has been detected based on the settings. Check your default Comm Port setting and make sure the USB adapter is plugged in.");
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
                _log.Debug("Starting live temperature read...");
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
                _log.Debug("Finished live temperature read!");
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
            using (var store = new InstrumentStore(_container))
            {
                await store.UpsertAsync(_instrument);
            }
        }

        private async Task ClearInstrumentValues()
        {
            await _instrumentCommunication.WriteItem(264, "20140867", false);
            await _instrumentCommunication.WriteItem(434, "0", false);
            await _instrumentCommunication.WriteItem(0, "0", false);
            await _instrumentCommunication.WriteItem(2, "0", false);
            await _instrumentCommunication.WriteItem(113, "0", false);
            await _instrumentCommunication.WriteItem(892, "0", false);
        }


        public async Task StartVolumeTest()
        {
            if (!_isBusy && !_runningTest)
            {
                await Task.Run(async () =>
                {
                    _log.Info("Starting volume test...");

                    //Reset Tach setting
                    if (!string.IsNullOrEmpty(_tachCommPort))
                    {
                        using (var tach = new TachometerCommunication(_tachCommPort))
                        {
                            await tach.ResetTach();
                        }
                    }

                    await ClearInstrumentValues();
                    await DownloadVolumeItems();
                    await SetProvingMode();
                    await _instrumentCommunication.Disconnect();

                    Instrument.Volume.PulseACount = 0;
                    Instrument.Volume.PulseBCount = 0;

                    OutputBoard.StartMotor();

                    _isBusy = false;
                    _runningTest = true;
                });
            }
        }

        private async Task SetProvingMode()
        {
            await _instrumentCommunication.WriteItem(433, "3");
        }

        public async Task StopVolumeTest()
        {
            if (_runningTest)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        _log.Info("Stopping volume test...");
                        OutputBoard?.StopMotor();
                        System.Threading.Thread.Sleep(500);

                        await DownloadVolumeAfterTestItems();
                        await ResetProvingMode();

                        await _instrumentCommunication.Disconnect();
                        Instrument.Volume.AppliedInput = await TachometerCommunication.ReadTachometer(_tachCommPort);
                        _log.Info("Volume test finished!");
                    }
                    finally
                    {
                        _isBusy = false;
                        _runningTest = false;
                    }
                });
            } 
        }

        private async Task ResetProvingMode()
        {
            await _instrumentCommunication.WriteItem(433, "1");
        }
    }
}
