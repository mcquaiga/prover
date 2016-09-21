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
        public static async Task<MechanicalTestManager> Create(IUnityContainer container, InstrumentType instrumentType, ICommPort instrumentPort, string tachometerPortName)
        {
            var instrumentComm = new InstrumentCommunicator(container.Resolve<IEventAggregator>(), instrumentPort, instrumentType);

            TachometerCommunicator tachComm = null;
            if (!string.IsNullOrEmpty(tachometerPortName))
            {
                tachComm = new TachometerCommunicator(tachometerPortName);
            }

            await commClient.Connect();
            var itemValues = await commClient.GetItemValues(commClient.ItemDetails.GetAllItemNumbers());
            await commClient.Disconnect();


            var instrument = new Instrument(InstrumentType.MiniAT, itemValues);
            var driveType = new MechanicalDrive(instrument);
            CreateVerificationTests(instrument, driveType);

            var volumeTest = instrument.VolumeTest;
            var volumeManager = new RotaryVolumeVerification(container.Resolve<IEventAggregator>(), volumeTest, instrumentComm, tachComm);

            var manager = new MechanicalTestManager(container, instrument, commClient, volumeManager, null);
            await manager.SaveAsync();
            container.RegisterInstance<TestManager>(manager);

            return manager;
        }
        public MechanicalTestManager(IUnityContainer container, Instrument instrument, InstrumentCommunicator instrumentComm, BaseVolumeVerificationManager volumeTestManager) 
            : base(container, instrument, instrumentComm)
        {
            VolumeTestManager = volumeTestManager;
        }
    }
}