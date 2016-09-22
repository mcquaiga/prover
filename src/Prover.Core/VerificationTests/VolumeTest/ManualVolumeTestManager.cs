using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;

namespace Prover.Core.VerificationTests.VolumeTest
{
    public sealed class ManualVolumeTestManager : VolumeTestManager
    {
        private IDInOutBoard _outputBoard;
        private TachometerCommunicator _tachometerCommunicator;
        public ManualVolumeTestManager(IEventAggregator eventAggregator, VolumeTest volumeTest, InstrumentCommunicator instrumentComm) 
            : base(eventAggregator, volumeTest, instrumentComm)
        {
            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
        }

        public override async Task PreTest()
        {
            if (!RunningTest)
            {
                await InstrumentCommunicator.Connect();
                VolumeTest.Items = await InstrumentCommunicator.GetItemValues(InstrumentCommunicator.ItemDetails.VolumeItems());
                await InstrumentCommunicator.Disconnect();

                VolumeTest.PulseACount = 0;
                VolumeTest.PulseBCount = 0;

                //Reset Tach setting
                //await _tachometerCommunicator?.ResetTach();

                _outputBoard.StartMotor();

                await ExecutingTest();
            }            
        }

        public override async Task ExecutingTest()
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

                _outputBoard?.StopMotor();
                await PostTest();

            });
        }

        public override async Task PostTest()
        {
            await Task.Run(async () =>
            {
                System.Threading.Thread.Sleep(250);

                VolumeTest.AppliedInput = await _tachometerCommunicator?.ReadTach();
                VolumeTest.AfterTestItems = await InstrumentCommunicator.GetItemValues(InstrumentCommunicator.ItemDetails.VolumeItems());
                await ZeroInstrumentVolumeItems();
                Log.Info("Volume test finished!");
                RunningTest = false;
            });
        }             
    }
}
