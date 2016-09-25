using System.Threading.Tasks;
using Caliburn.Micro;
using MccDaq;
using NLog;
using Prover.CommProtocol.Common;
using Prover.Core.ExternalDevices.DInOutBoards;
using LogManager = Caliburn.Micro.LogManager;

namespace Prover.Core.VerificationTests.VolumeVerification
{
    public abstract class VolumeTestManager
    {
        protected IEventAggregator EventAggreator;
        protected IDInOutBoard FirstPortAInputBoard;
        protected IDInOutBoard FirstPortBInputBoard;
        protected EvcCommunicationClient InstrumentCommunicator;
        protected bool IsFirstVolumeTest = true;
        protected Logger Log = LogManager.GetCurrentClassLogger();
        protected bool RequestStopTest;
        protected bool RunningTest = false;

        protected VolumeTestManager(
            IEventAggregator eventAggregator,
            IDAQInputService daqInputService)
        {
            EventAggreator = eventAggregator;

            FirstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            FirstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);
        }

        public virtual void StopVolumeTest()
        {
            RequestStopTest = true;
        }

        public abstract Task PreTest();
        public abstract Task ExecutingTest();
        public abstract Task PostTest();
    }
}