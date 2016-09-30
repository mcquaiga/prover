using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.Events;
using Prover.Core.Models;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Core.Communication;
using Prover.SerialProtocol;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Prover.Core.DriveTypes;
using Prover.Core.EVCTypes;
using Prover.Core.VerificationTests.Rotary;

namespace Prover.Core.VerificationTests
{
    public class RotaryTestManager : TestManager
    {
        public static async Task<RotaryTestManager> CreateRotaryTest(IUnityContainer container, InstrumentType instrumentType, ICommPort instrumentPort, string tachometerPortName)
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

            var driveType = new RotaryDrive(instrument);
            CreateVerificationTests(instrument, driveType);

            var volumeTest = instrument.VerificationTests.FirstOrDefault(x => x.VolumeTest != null).VolumeTest;
            var rotaryVolumeTest = new RotaryVolumeVerification(container.Resolve<IEventAggregator>(), volumeTest, instrumentComm, tachComm);

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
