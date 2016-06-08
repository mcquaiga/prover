using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.Mechanical
{
    public sealed class MechanicalVolumeVerification : BaseVolumeVerificationManager
    {
        public MechanicalVolumeVerification(IEventAggregator eventAggregator, VolumeTest volumeTest, EvcCommunicationClient commClient) 
            : base(eventAggregator, volumeTest, commClient)
        {
        }

        public override async Task StartVolumeTest()
        {
            if (!_runningTest)
            {
                await _instrumentCommunicator.Connect();
                VolumeTest.Items = await _instrumentCommunicator.GetItemValues(_instrumentCommunicator.ItemDetails.VolumeItems());
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
                    VolumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                    VolumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
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

                    VolumeTest.AfterTestItems = await _instrumentCommunicator.GetItemValues(_instrumentCommunicator.ItemDetails.VolumeItems());

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
