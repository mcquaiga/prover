using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests
{
    public sealed class MechanicalVolumeVerification : BaseVolumeVerificationManager
    {
        public MechanicalVolumeVerification(IEventAggregator eventAggregator, VolumeTest volumeTest, InstrumentCommunicator instrumentComm) 
            : base(eventAggregator, volumeTest, instrumentComm)
        {
        }

        public override async Task StartVolumeTest()
        {
            if (!_runningTest)
            {
                VolumeTest.ItemValues = await _instrumentCommunicator.DownloadItemsAsync(_volumeItems);
                await _instrumentCommunicator.Disconnect();

                VolumeTest.PulseACount = 0;
                VolumeTest.PulseBCount = 0;
                
                await RunningVolumeTest();
            }
            
        }

        public override async Task RunningVolumeTest()
        {
            await Task.Run(async () =>
            {
                _runningTest = true;
                do
                {
                    //TODO: Raise events so the UI can respond
                    VolumeTest.PulseACount += _firstPortAInputBoard.ReadInput();
                    VolumeTest.PulseBCount += _firstPortBInputBoard.ReadInput();
                } while (VolumeTest.UncPulseCount < VolumeTest.DriveType.MaxUnCorrected() && !_requestStopTest);
                await FinishVolumeTest();
            });
        }

        public override async Task FinishVolumeTest()
        {
            await Task.Run(async () =>
            {
                try
                {
                    System.Threading.Thread.Sleep(250);

                    VolumeTest.AfterTestItemValues = await _instrumentCommunicator.DownloadItemsAsync(_volumeItems);

                    await ZeroInstrumentVolumeItems();

                    _log.Info("Volume test finished!");
                }
                finally
                {
                    _runningTest = false;
                }
            });
        }             
    }
}
