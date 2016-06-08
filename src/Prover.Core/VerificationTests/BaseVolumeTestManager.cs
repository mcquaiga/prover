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
        protected IDInOutBoard FirstPortAInputBoard;
        protected IDInOutBoard FirstPortBInputBoard;
        protected EvcCommunicationClient _instrumentCommunicator;
        protected bool _isFirstVolumeTest = true;
        protected Logger _log = LogManager.GetCurrentClassLogger();
        protected bool _requestStopTest;
        protected bool _runningTest = false;
        protected IEventAggregator EventAggreator;

        protected BaseVolumeVerificationManager(IEventAggregator eventAggregator, VolumeTest volumeTest,
            EvcCommunicationClient instrumentComm)
        {
            EventAggreator = eventAggregator;
            _instrumentCommunicator = instrumentComm;

            VolumeTest = volumeTest;

            FirstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            FirstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);
        }

        public VolumeTest VolumeTest { get; private set; }

        public virtual void StopVolumeTest()
        {
            _requestStopTest = true;
        }

        protected virtual async Task ZeroInstrumentVolumeItems()
        {
            await _instrumentCommunicator.WriteItem(0, "0", false);
            await _instrumentCommunicator.WriteItem(2, "0", false);
        }

        public abstract Task StartVolumeTest();
        public abstract Task RunningVolumeTest();
        public abstract Task FinishVolumeTest();
    }
}