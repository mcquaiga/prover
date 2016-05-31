using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.CommClients;
using Prover.Core.Communication;
using Prover.Core.DriveTypes;
using Prover.Core.Models;
using Prover.Core.Models.Instruments;
using Prover.SerialProtocol;

namespace Prover.Core.VerificationTests.Rotary
{
    public class RotaryTestManager : TestManager
    {
        public static async Task<RotaryTestManager> CreateRotaryTest(IUnityContainer container, InstrumentType instrumentType, CommPort instrumentPort, string tachometerPortName)
        {

            TachometerCommunicator tachComm = null;
            if (!string.IsNullOrEmpty(tachometerPortName))
            {
                tachComm = new TachometerCommunicator(tachometerPortName);
            }

            var client = new MiniMaxClient(instrumentPort);
            var itemValues = await client.GetItemValue(client.ItemDetails.GetAllItemNumbers());
            var instrument = new Instrument(instrumentType, client.ItemDetails.GetAllItemNumbers(), itemValues);

            var driveType = new RotaryDrive(instrument);
            CreateVerificationTests(instrument, driveType);

            var volumeTest = instrument.VerificationTests.FirstOrDefault(x => x.VolumeTest != null).VolumeTest;
            var rotaryVolumeTest = new RotaryVolumeVerification(container, volumeTest, instrumentComm, tachComm);

            var manager = new RotaryTestManager(container, instrument, instrumentComm, rotaryVolumeTest);
            container.RegisterInstance<TestManager>(manager);

            return manager;
        }

        public RotaryTestManager(IUnityContainer container, Instrument instrument, InstrumentCommunicator instrumentCommunicator, RotaryVolumeVerification volumeTestManager)
            :base(container, instrument, instrumentCommunicator)
        {
            VolumeTestManager = volumeTestManager as RotaryVolumeVerification;
        }

   
    }
}
