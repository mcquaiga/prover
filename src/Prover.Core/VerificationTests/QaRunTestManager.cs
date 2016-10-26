using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.DriveTypes;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.VolumeVerification;

namespace Prover.Core.VerificationTests
{
    public interface IQaRunTestManager
    {
        Instrument Instrument { get; }
        void Dispose();
        Task InitializeTest(InstrumentType instrumentType, IDriveType driveType);
        Task RunTest(int level);
        Task DownloadVerificationTestItems(int level);
        Task DownloadTemperatureTestItems(int levelNumber);
        Task DownloadPressureTestItems(int level);
        Task SaveAsync();
        Task RunVerifier();
    }

    public class QaRunTestManager : IDisposable, IQaRunTestManager
    {
        private const int VolumeTestNumber = 0;
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        private readonly EvcCommunicationClient _communicationClient;
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private readonly IReadingStabilizer _readingStabilizer;
        private readonly IEnumerable<IVerifier> _verifiers;

        public QaRunTestManager(
            IInstrumentStore<Instrument> instrumentStore,
            EvcCommunicationClient commClient,
            IReadingStabilizer readingStabilizer
            )
        {
            _instrumentStore = instrumentStore;
            _communicationClient = commClient;
            _readingStabilizer = readingStabilizer;
            
        }

        public VolumeTestManagerBase VolumeTestManagerBase { get; set; }
        
        public Instrument Instrument { get; private set; }

        public async Task InitializeTest(InstrumentType instrumentType, IDriveType driveType)
        {
            _communicationClient.Initialize(instrumentType);
            await _communicationClient.Connect();
            var items = await _communicationClient.GetItemValues(_communicationClient.ItemDetails.GetAllItemNumbers());
            Instrument = new Instrument(instrumentType, driveType, items);
            await _communicationClient.Disconnect();
            await SaveAsync();
            await RunVerifier();
        }

        public async Task RunTest(int level)
        {
            if (Instrument == null) throw new NullReferenceException("Call InitializeTest before runnning a test");

            await _readingStabilizer.WaitForReadingsToStabilizeAsync(_communicationClient, Instrument, level);
            await DownloadVerificationTestItems(level);

            if (Instrument.VolumeTest != null)
                await VolumeTestManagerBase.RunTest(_communicationClient, Instrument.VolumeTest, null);
        }

        public async Task DownloadVerificationTestItems(int level)
        {
            if (!_communicationClient.IsConnected)
                await _communicationClient.Connect();

            if (Instrument.CompositionType == CorrectorType.PTZ)
            {
                await DownloadTemperatureTestItems(level);
                await DownloadPressureTestItems(level);
            }

            if (Instrument.CompositionType == CorrectorType.T)
                await DownloadTemperatureTestItems(level);

            if (Instrument.CompositionType == CorrectorType.P)
                await DownloadPressureTestItems(level);

            await _communicationClient.Disconnect();
        }

        public async Task DownloadTemperatureTestItems(int levelNumber)
        {
            var test = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber).TemperatureTest;

            if (test != null)
                test.Items =
                    await _communicationClient.GetItemValues(_communicationClient.ItemDetails.TemperatureItems());
        }

        public async Task DownloadPressureTestItems(int level)
        {
            var test = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level).PressureTest;
            if (test != null)
                test.Items = await _communicationClient.GetItemValues(_communicationClient.ItemDetails.PressureItems());
        }

        public async Task SaveAsync()
        {
            try
            {
                await _instrumentStore.UpsertAsync(Instrument);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public async Task RunVerifier()
        {
            var verifiers = _verifiers.ToArray();

            if (verifiers.Any())
                foreach (var verifier in verifiers)
                    await verifier.Verify(_communicationClient, Instrument);
        }

        public void Dispose()
        {
            _communicationClient.Dispose();
        }
    }
}