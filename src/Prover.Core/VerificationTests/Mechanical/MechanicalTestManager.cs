using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.DriveTypes;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.Mechanical
{
    public sealed class MechanicalTestManager : TestManager
    {
        public MechanicalTestManager(IUnityContainer container, Instrument instrument,
            EvcCommunicationClient instrumentComm, MechanicalVolumeVerification volumeTestManager, IVerifier verifier)
            : base(container, instrument, instrumentComm, verifier)
        {
            VolumeTestManager = volumeTestManager;
        }

        public static async Task<MechanicalTestManager> Create(IUnityContainer container, EvcCommunicationClient commClient)
        {
            await commClient.Connect();
            var itemValues = await commClient.GetItemValues(commClient.ItemDetails.GetAllItemNumbers());
            await commClient.Disconnect();

            var instrument = new Instrument(InstrumentType.MiniAT, itemValues);
            var driveType = new MechanicalDrive(instrument);
            CreateVerificationTests(instrument, driveType);

            var volumeTest = instrument.VolumeTest;
            var volumeManager = new MechanicalVolumeVerification(container.Resolve<IEventAggregator>(), volumeTest,
                commClient);

            var manager = new MechanicalTestManager(container, instrument, commClient, volumeManager, null);
            container.RegisterInstance<TestManager>(manager);

            return manager;
        }
    }
}