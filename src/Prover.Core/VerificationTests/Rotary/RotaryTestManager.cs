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
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.Rotary
{
    public sealed class RotaryTestManager : TestManager
    {
        private RotaryTestManager(IUnityContainer container, Instrument instrument,
            EvcCommunicationClient instrumentCommunicator, RotaryVolumeVerification volumeTestManager)
            : base(container, instrument, instrumentCommunicator)
        {
            VolumeTestManager = volumeTestManager;
        }

        public static async Task<RotaryTestManager> CreateRotaryTest(IUnityContainer container, EvcCommunicationClient instrumentCommClient, string tachometerPortName)
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

            var manager = new RotaryTestManager(container, instrument, instrumentCommClient, rotaryVolumeTest);
            container.RegisterInstance<TestManager>(manager);

            return manager;
        }
    }
}