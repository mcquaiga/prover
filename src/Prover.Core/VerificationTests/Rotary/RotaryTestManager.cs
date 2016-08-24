using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Communication;
using Prover.Core.DriveTypes;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.Rotary
{
    public sealed class RotaryTestManager : TestManager
    {
        private RotaryTestManager(IUnityContainer container, Instrument instrument,
            EvcCommunicationClient instrumentCommunicator, VolumeVerificationManager volumeTestManager, IVerifier verifier)
            : base(container, instrument, instrumentCommunicator, verifier)
        {
            VolumeTestManager = volumeTestManager;
        }

        public static async Task<RotaryTestManager> CreateRotaryTest(IUnityContainer container, EvcCommunicationClient instrumentCommClient, string tachometerPortName, IVerifier verifier)
        {
            TachometerCommunicator tachComm = null;
            if (!string.IsNullOrEmpty(tachometerPortName))
            {
                tachComm = new TachometerCommunicator(tachometerPortName);
            }

            await instrumentCommClient.Connect();
            var itemValues = await instrumentCommClient.GetItemValues(instrumentCommClient.ItemDetails.GetAllItemNumbers());
            await instrumentCommClient.Disconnect();

            var instrument = new Instrument(InstrumentType.MiniMax, itemValues);
            var driveType = new RotaryDrive(instrument);
            CreateVerificationTests(instrument, driveType);

            var volumeTest = instrument.VolumeTest;
            var rotaryVolumeTest = new RotaryVolumeVerification(container.Resolve<IEventAggregator>(), volumeTest, instrumentCommClient, tachComm);

            var manager = new RotaryTestManager(container, instrument, instrumentCommClient, rotaryVolumeTest, verifier);
            await manager.SaveAsync();
            container.RegisterInstance<TestManager>(manager);

            return manager;
        }
    }
}