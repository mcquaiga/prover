using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.Events;
using Prover.Core.Models;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Core.VerificationTests;
using Prover.SerialProtocol;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Prover.Core.Communication
{
    public class TestManager
    {
        private Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly IUnityContainer _container;
        private bool _isLiveReading = false;
        private bool _stopLiveReading;
        private bool _isBusy = false;
        
        public Instrument Instrument { get; private set; }
        public InstrumentCommunicator InstrumentCommunicator { get; private set; }
        public TachometerCommunicator TachometerCommunicator { get; private set; }
        private RotaryVolumeVerification VolumeTest { get; }

        public static async Task<TestManager> Create(IUnityContainer container, InstrumentType instrumentType, ICommPort instrumentPort, string tachometerPortName)
        {
            var instrumentComm = new InstrumentCommunicator(instrumentPort, instrumentType);
            var tachComm = new TachometerCommunicator(tachometerPortName);

            var items = new InstrumentItems(instrumentType);
            await instrumentComm.DownloadItemsAsync(items);
            var instrument = new Instrument(instrumentType, items);

            return new TestManager(container, instrument, instrumentComm, tachComm);
        }

        private TestManager(IUnityContainer container, Instrument instrument, InstrumentCommunicator instrumentCommunicator, TachometerCommunicator tachCommunicator) 
        {
            _container = container;
            Instrument = instrument;
            InstrumentCommunicator = instrumentCommunicator;
            TachometerCommunicator = tachCommunicator;

            VolumeTest = new RotaryVolumeVerification(instrument, instrumentCommunicator, tachCommunicator);
        }

        public async Task DownloadVerificationTestItems(int level)
        {
            if (Instrument.CorrectorType == CorrectorType.PressureTemperature || Instrument.CorrectorType == CorrectorType.TemperatureOnly)
            {
                await DownloadTemperatureTestItems((TemperatureTest.Level)level);
            }

            if (Instrument.CorrectorType == CorrectorType.PressureTemperature || Instrument.CorrectorType == CorrectorType.PressureOnly)
            {
                await DownloadPressureTestItems((PressureTest.PressureLevel)level);
            }
        }

        public async Task DownloadTemperatureTestItems(TemperatureTest.Level level)
        {
            if (_isLiveReading) await StopLiveRead();

            var test = Instrument.Temperature.Tests.FirstOrDefault(x => x.TestLevel == level);
            if (test != null)
                test.Items.InstrumentValues = await InstrumentCommunicator.DownloadItemsAsync(test.Items.Items);
  
            _isBusy = false;
        }

        public async Task DownloadPressureTestItems(PressureTest.PressureLevel level)
        {
            if (_isLiveReading) await StopLiveRead();

            var test = Instrument.Pressure.Tests.FirstOrDefault(x => x.TestLevel == level);
            if (test != null)
                test.Items.InstrumentValues = await InstrumentCommunicator.DownloadItemsAsync(test.Items.Items);

            _isBusy = false;
        }

        public async Task StartLiveRead(int itemNumber)
        {
            if (!_isBusy)
            {
                _log.Debug("Starting live read...");
                await InstrumentCommunicator.Connect();
                _stopLiveReading = false;
                _isLiveReading = true;
                do
                {
                    var liveValue = await InstrumentCommunicator.LiveReadItem(itemNumber);
                    _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new LiveReadEvent(liveValue));
                } while (!_stopLiveReading);

                await InstrumentCommunicator.Disconnect();
                _isLiveReading = false;
                _isBusy = false;
                _log.Debug("Finished live read!");
            } 
        }

        public async Task StopLiveRead()
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
            await VolumeTest.BeginVerificationTest();
        }

        public void StopVolumeTest()
        {
            VolumeTest.StopRunningTest();
        }
    }
}
