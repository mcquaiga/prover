namespace Prover.Core.ExternalDevices.DInOutBoards
{
    using MccDaq;
    using NLog;
    using System;

    /// <summary>
    /// Defines the <see cref="DataAcqBoard" />
    /// </summary>
    public class DataAcqBoard : IDisposable, IDInOutBoard
    {
        #region Constants

        /// <summary>
        /// Defines the PulseTimingDefaultSeconds
        /// </summary>
        private const decimal PulseTimingDefaultSeconds = 0.0625m;

        #endregion

        #region Fields

        /// <summary>
        /// Defines the _channelNum
        /// </summary>
        private readonly int _channelNum;

        /// <summary>
        /// Defines the _channelType
        /// </summary>
        private readonly DigitalPortType _channelType;

        /// <summary>
        /// Defines the _log
        /// </summary>
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Defines the _board
        /// </summary>
        private MccBoard _board;

        /// <summary>
        /// Defines the _pulseIsCleared
        /// </summary>
        private bool _pulseIsCleared;

        /// <summary>
        /// Defines the _ulStatErrorInfo
        /// </summary>
        private ErrorInfo _ulStatErrorInfo;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcqBoard"/> class.
        /// </summary>
        /// <param name="boardNumber">The boardNumber<see cref="int"/></param>
        /// <param name="channelType">The channelType<see cref="DigitalPortType"/></param>
        /// <param name="channelNumber">The channelNumber<see cref="int"/></param>
        public DataAcqBoard(int boardNumber, DigitalPortType channelType, int channelNumber)
        {
            _ulStatErrorInfo = MccService.ErrHandling(ErrorReporting.PrintAll, ErrorHandling.DontStop);
            
            string boardName = string.Empty;
            var errorInfo = MccService.GetBoardName(boardNumber, ref boardName);
            if (errorInfo.Value == ErrorInfo.ErrorCode.BadBoard)
                throw new Exception("Board not found");
            
            _board = new MccBoard(boardNumber);

            _channelType = channelType;
            _channelNum = channelNumber;

            _log.Info("Initialized DataAcqBoard: {0}, channel type {1}, channel number {2}", boardNumber, channelType, channelNumber);

            //var boardStatus = _board.GetStatus(out var status, out var curCount, out var curIndex, FunctionType.DaqoFunction);
            //if (boardStatus.Value != ErrorInfo.ErrorCode.NoErrors)
            //{
            //    throw new Exception("DAQ board could not be found or is not configured correctly.");
            //}
        }

        #endregion

        #region Enums

        private enum MotorValues
        {
            Start = 1023,
            Stop = 0
        }

        private enum OutputPorts
        {
            DaOut0 = 0,
            DaOut1 = 1
        }

        private enum SignalValues
        {
            On = 254,
            Off = 255
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the InputValue
        /// </summary>
        public short InputValue { get; private set; }

        /// <summary>
        /// Gets or sets the PulseTiming
        /// </summary>
        public decimal PulseTiming { get; set; } = PulseTimingDefaultSeconds;

        #endregion

        #region Methods

        /// <summary>
        /// The Dispose
        /// </summary>
        public void Dispose()
        {
            _board = null;
        }

        /// <summary>
        /// The ReadInput
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public int ReadInput()
        {

            _ulStatErrorInfo = _board.DIn(_channelType, out short value);

            if (_ulStatErrorInfo.Value == ErrorInfo.ErrorCode.NoErrors)
            {
                if (value != 255)
                {
                    if (_pulseIsCleared)
                    {
                        _log.Trace($"Pulse value read -> value = {value}");
                        _pulseIsCleared = false;
                        return 1;
                    }
                }
                else
                {
                    _pulseIsCleared = true;
                }
            }
            else
            {
                if (_ulStatErrorInfo.Value != ErrorInfo.ErrorCode.BadBoard)
                    _log.Warn("DAQ Input error: {0} - {1}", _ulStatErrorInfo.Message, _ulStatErrorInfo.Value);
            }
            return 0;
        }

        /// <summary>
        /// The StartMotor
        /// </summary>
        public void StartMotor()
        {
            Out(MotorValues.Start);
        }

        /// <summary>
        /// The StopMotor
        /// </summary>
        public void StopMotor()
        {
            Out(MotorValues.Stop);
        }

        /// <summary>
        /// The Out
        /// </summary>
        /// <param name="outputValue">The outputValue<see cref="MotorValues"/></param>
        private void Out(MotorValues outputValue)
        {
            _ulStatErrorInfo = _board?.AOut(_channelNum, Range.UniPt05Volts, (short)outputValue);

            if (_ulStatErrorInfo?.Value != ErrorInfo.ErrorCode.NoErrors && _ulStatErrorInfo?.Value != ErrorInfo.ErrorCode.BadBoard)
                _log.Warn("DAQ Output error: {0}", _ulStatErrorInfo?.Message);
        }

        #endregion
    }
}
