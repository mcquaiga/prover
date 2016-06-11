using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.Mechanical
{
    public sealed class MechanicalVolumeVerification : VolumeVerificationManager
    {
        public MechanicalVolumeVerification(IEventAggregator eventAggregator, VolumeTest volumeTest, EvcCommunicationClient commClient) 
            : base(eventAggregator, volumeTest, commClient)
        {
        }

        public override async Task StartVolumeTest()
        {
            if (!RunningTest)
            {
                await InstrumentCommunicator.Connect();
                VolumeTest.Items = await InstrumentCommunicator.GetItemValues(InstrumentCommunicator.ItemDetails.VolumeItems());
                await InstrumentCommunicator.Disconnect();

                VolumeTest.PulseACount = 0;
                VolumeTest.PulseBCount = 0;
                
                await RunningVolumeTest();
            }
            
        }

        public override async Task RunningVolumeTest()
        {
            await Task.Run(async () =>
            {
                RunningTest = true;
                do
                {
                    //TODO: Raise events so the UI can respond
                    VolumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                    VolumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                } while (VolumeTest.UncPulseCount < VolumeTest.DriveType.MaxUnCorrected() && !RequestStopTest);

                await FinishVolumeTest();
            });
        }

        public override async Task FinishVolumeTest()
        {
            await Task.Run(async () =>
            {
                System.Threading.Thread.Sleep(250);

                await InstrumentCommunicator.Connect();
                VolumeTest.AfterTestItems = await InstrumentCommunicator.GetItemValues(InstrumentCommunicator.ItemDetails.VolumeItems());
                await ZeroInstrumentVolumeItems();

                Log.Info("Volume test finished!");
                RunningTest = false;
            });
        }             
    }
}
