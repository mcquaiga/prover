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
using Prover.Core.Models;

namespace Prover.Core.Communication
{
    public class TestManager
    {
        private Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly IUnityContainer _container;
        private bool _isLiveReading = false;
        private bool _stopLiveReading;

        private bool _runningTest = false;
        private bool _isBusy = false;
        private bool _isFirstVolumeTest;
        private ICommPort _instrumentCommPort;

        public DataAcqBoard OutputBoard { get; private set; } = new DataAcqBoard(0, 0, 0);
        public DataAcqBoard AInputBoard { get; private set; } = new DataAcqBoard(0, DigitalPortType.FirstPortA, 0);
        public DataAcqBoard BInputBoard { get; private set; } = new DataAcqBoard(0, DigitalPortType.FirstPortB, 1);

        public Instrument Instrument { get; private set; }
        public InstrumentCommunicator InstrumentCommunicator { get; private set; }
        public TachometerCommunicator TachometerCommunicator { get; private set; }

        private TestManager(IUnityContainer container)
        {
            _container = container;
        }

        public TestManager(IUnityContainer container, InstrumentCommunicator instrumentCommunicator, TachometerCommunicator tachCommunicator) 
            : this(container)
        {

            InstrumentCommunicator = instrumentCommunicator;
            TachometerCommunicator = tachCommunicator;
        }

        public TestManager(IUnityContainer container, ICommPort instrumentPort, string tachometerPortName)
            : this(container)
        {
            _instrumentCommPort = instrumentPort;

            if (!string.IsNullOrEmpty(tachometerPortName))
                TachometerCommunicator = new TachometerCommunicator(tachometerPortName);
        }

        public async Task InitializeInstrument(InstrumentType instrumentType)
        {
            if (InstrumentCommunicator == null)
                InstrumentCommunicator = new InstrumentCommunicator(_instrumentCommPort, instrumentType);

            var items = new InstrumentItems(instrumentType);
            Instrument.Items.InstrumentValues = await InstrumentCommunicator.DownloadItemsAsync(Instrument.Items.Items);
            Instrument = new Instrument(instrumentType, items);
        }

        public async Task DownloadTemperatureTestItems(TemperatureTest.Level level)
        {
            if (_isLiveReading) await StopLiveReadTemperature();

            var test = Instrument.Temperature.Tests.FirstOrDefault(x => x.TestLevel == level);
            if (test != null)
                test.Items.InstrumentValues = await InstrumentCommunicator.DownloadItemsAsync(test.Items.Items);
  
            _isBusy = false;
        }

        public async Task StartLiveReadTemperature()
        {
            if (!_isBusy)
            {
                _log.Debug("Starting live temperature read...");
                await InstrumentCommunicator.Connect();
                _stopLiveReading = false;
                _isLiveReading = true;
                do
                {
                    var liveValue = await InstrumentCommunicator.LiveReadItem(26);
                    _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new LiveReadEvent(liveValue));
                } while (!_stopLiveReading);

                await InstrumentCommunicator.Disconnect();
                _isLiveReading = false;
                _isBusy = false;
                _log.Debug("Finished live temperature read!");
            } 
        }

        public async Task StopLiveReadTemperature()
        {
            if (_isLiveReading)
            {
                _stopLiveReading = true;
                await InstrumentCommunicator.Disconnect();
            }
        }

        public async Task SaveAsync()
        {
            var store = new InstrumentStore(_container);
            await store.UpsertAsync(Instrument);
        }

        public async Task StartVolumeTest()
        {
            if (!_isBusy && !_runningTest)
            {
                await Task.Run(async () =>
                {
                    if (!_isFirstVolumeTest)
                        await InstrumentCommunicator.DownloadItemsAsync(Instrument.Volume.Items);

                    _isFirstVolumeTest = false;

                    _log.Info("Starting volume test...");

                    await InstrumentCommunicator.Disconnect();

                    //Reset Tach setting
                    await TachometerCommunicator?.ResetTach();

                    Instrument.Volume.PulseACount = 0;
                    Instrument.Volume.PulseBCount = 0;

                    OutputBoard.StartMotor();

                    _isBusy = false;
                    _runningTest = true;
                });
            }
        }

        public async Task StopVolumeTest()
        {
            if (!_isBusy)
            {
                await Task.Run(async () =>
                {
                    _log.Info("Stopping volume test...");
                    try
                    {
                        OutputBoard?.StopMotor();

                        System.Threading.Thread.Sleep(500);

                        await InstrumentCommunicator.DownloadItemsAsync(Instrument.Volume.AfterTestItems);
                        
                        try
                        {
                            Instrument.Volume.AppliedInput = await TachometerCommunicator?.ReadTach();
                            _log.Info(string.Format("Tachometer reading: {0}", Instrument.Volume?.AppliedInput));
                        }
                        catch (Exception ex)
                        {
                            _log.Error(string.Format("An error occured communication with the tachometer: {0}", ex));
                        }
                                      
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
    }
}
