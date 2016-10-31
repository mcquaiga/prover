//using System;
//using System.Threading.Tasks;
//using Caliburn.Micro;
//using NLog;
//using NLog.Fluent;
//using Prover.CommProtocol.Common;
//using Prover.CommProtocol.Common.Items;
//using Prover.Core.Communication;
//using Prover.Core.ExternalDevices.DInOutBoards;
//using Prover.Core.Models.Instruments;
//using LogManager = NLog.LogManager;

//namespace Prover.Core.VerificationTests.VolumeVerification
//{
//    public sealed class ManualVolumeTestManager : VolumeTestManagerBase
//    {
//        private IDInOutBoard _outputBoard;
//        private TachometerCommunicator _tachometerCommunicator;
//        private bool _requestStopTest;

//        public ManualVolumeTestManager(IEventAggregator eventAggregator) : base(eventAggregator)
//        {
//            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
//        }

//        protected override Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest)
//        {
//            throw new NotImplementedException();
//        }

//        protected override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, IEvcItemReset evcTestItemReset)
//        {
//            await commClient.Connect();
//            await evcTestItemReset.PreReset(commClient);
//            volumeTest.Items = await commClient.GetItemValues(commClient.ItemDetails.VolumeItems());
//            await commClient.Disconnect();

//            volumeTest.PulseACount = 0;
//            volumeTest.PulseBCount = 0;        
//        }

//        protected override async Task ExecutingTest(VolumeTest volumeTest)
//        {
//            await Task.Run(() =>
//            {
//                do
//                {
//                    //TODO: Raise events so the UI can respond
//                    //volumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
//                    //volumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
//                } while (volumeTest.UncPulseCount < volumeTest.DriveType.MaxUncorrectedPulses() && !_requestStopTest);

//                _outputBoard?.StopMotor();
//            });
//        }

//        protected override async Task PostTest(EvcCommunicationClient commClient, VolumeTest volumeTest, IEvcItemReset evcPostTestItemReset)
//        {
//            await Task.Run(async () =>
//            {
//                try
//                {
//                    await commClient.Connect();
//                    volumeTest.AfterTestItems = await commClient.GetItemValues(commClient.ItemDetails.VolumeItems());
//                    await evcPostTestItemReset.PostReset(commClient);
//                }
//                catch (Exception ex)
//                {
//                    NLog.LogManager.GetCurrentClassLogger().Error(ex, "Error in Post volume test");
//                }
//                finally
//                {
//                    await commClient.Disconnect();
//                    NLog.LogManager.GetCurrentClassLogger().Info("Volume test finished!");
//                }
//            });
//        }             
//    }
//}
