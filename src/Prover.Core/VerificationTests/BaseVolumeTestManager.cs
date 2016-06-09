using System.Threading.Tasks;
using Caliburn.Micro;
using MccDaq;
using NLog;
using Prover.CommProtocol.Common;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Instruments;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests
{
    public abstract class BaseVolumeVerificationManager
    {
        protected IEventAggregator EventAggreator;
        protected IDInOutBoard FirstPortAInputBoard;
        protected IDInOutBoard FirstPortBInputBoard;
        protected EvcCommunicationClient InstrumentCommunicator;
        protected bool IsFirstVolumeTest = true;
        protected Logger Log = LogManager.GetCurrentClassLogger();
        protected bool RequestStopTest;
        protected bool RunningTest = false;

        protected BaseVolumeVerificationManager(IEventAggregator eventAggregator, VolumeTest volumeTest,
            EvcCommunicationClient instrumentComm)
        {
            EventAggreator = eventAggregator;
            InstrumentCommunicator = instrumentComm;
            VolumeTest = volumeTest;

            FirstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            FirstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);
        }

        public VolumeTest VolumeTest { get; private set; }

        public virtual void StopVolumeTest()
        {
            RequestStopTest = true;
        }

        protected virtual async Task ZeroInstrumentVolumeItems()
        {
            if (!InstrumentCommunicator.IsConnected)
                await InstrumentCommunicator.Connect();

            await InstrumentCommunicator.SetItemValue(0, "0");
            await InstrumentCommunicator.SetItemValue(2, "0");
            await InstrumentCommunicator.Disconnect();
        }

        public abstract Task StartVolumeTest();
        public abstract Task RunningVolumeTest();
        public abstract Task FinishVolumeTest();
    }
}