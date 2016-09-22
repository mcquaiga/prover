using System.Threading.Tasks;
using Caliburn.Micro;
using MccDaq;
using NLog;
using Prover.CommProtocol.Common;
using Prover.Core.ExternalDevices.DInOutBoards;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests.VolumeTest
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

        protected VolumeTestManager(IEventAggregator eventAggregator, Models.Instruments.VolumeTest volumeTest,
            EvcCommunicationClient instrumentComm)
        {
            EventAggreator = eventAggregator;
            InstrumentCommunicator = instrumentComm;
            VolumeTest = volumeTest;

            FirstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            FirstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);
        }

        public Models.Instruments.VolumeTest VolumeTest { get; private set; }

        public virtual void StopVolumeTest()
        {
            RequestStopTest = true;
        }

        protected virtual async Task ZeroInstrumentVolumeItems()
        {
            await InstrumentCommunicator.Connect();
            await InstrumentCommunicator.SetItemValue(0, "0");
            await InstrumentCommunicator.SetItemValue(2, "0");
            await InstrumentCommunicator.Disconnect();
        }

        public abstract Task PreTest();
        public abstract Task ExecutingTest();
        public abstract Task PostTest();
    }
}