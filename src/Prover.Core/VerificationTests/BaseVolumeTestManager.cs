using Caliburn.Micro;
using MccDaq;
using NLog;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models;
using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.VerificationTests
{
    public abstract class BaseVolumeVerificationManager
    {
        protected Logger _log = NLog.LogManager.GetCurrentClassLogger();
        protected IEventAggregator _eventAggreator;
        protected InstrumentCommunicator _instrumentCommunicator;
        protected IDInOutBoard _firstPortAInputBoard;
        protected IDInOutBoard _firstPortBInputBoard;
        protected List<ItemDetail> _volumeItems;
        protected bool _requestStopTest;
        protected bool _isFirstVolumeTest = true;
        protected bool _runningTest = false;

        protected BaseVolumeVerificationManager(IEventAggregator eventAggregator, VolumeTest volumeTest, InstrumentCommunicator instrumentComm)
        {
            _eventAggreator = eventAggregator;
            _instrumentCommunicator = instrumentComm;

            VolumeTest = volumeTest;
            _volumeItems = volumeTest.VerificationTest.Instrument.Items.Items.Where(i => i.IsVolumeTest == true).ToList();

            _firstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            _firstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);
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
