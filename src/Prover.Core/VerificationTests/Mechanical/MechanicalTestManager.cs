using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.SerialProtocol;
using Caliburn.Micro;
using Prover.Core.DriveTypes;
using Prover.Core.Models;
using Prover.Core.EVCTypes;

namespace Prover.Core.VerificationTests.Mechanical
{
    public class MechanicalTestManager : TestManager
    {
        public static async Task<MechanicalTestManager> Create(IUnityContainer container, InstrumentType instrumentType, ICommPort instrumentPort, string tachometerPortName)
        {
            var instrumentComm = new InstrumentCommunicator(container.Resolve<IEventAggregator>(), instrumentPort, instrumentType);

            TachometerCommunicator tachComm = null;
            if (!string.IsNullOrEmpty(tachometerPortName))
            {
                tachComm = new TachometerCommunicator(tachometerPortName);
            }

            var items = new InstrumentItems(instrumentType);
            var itemValues = await instrumentComm.DownloadItemsAsync(items.Items.ToList());
            var instrument = new Instrument(instrumentType, items, itemValues);

            var driveType = new MechanicalDrive(instrument);
            CreateVerificationTests(instrument, driveType);

            var volumeTest = instrument.VerificationTests.FirstOrDefault(x => x.VolumeTest != null).VolumeTest;
            var volumeManager = new RotaryVolumeVerification(container.Resolve<IEventAggregator>(), volumeTest, instrumentComm, tachComm);

            var manager = new MechanicalTestManager(container, instrument, instrumentComm, volumeManager);
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
