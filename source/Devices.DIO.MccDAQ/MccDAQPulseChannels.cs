using MccDaq;
using Microsoft.Extensions.Logging;
using Prover.Application;
using Prover.Shared;
using Prover.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;

namespace Prover.Modules.DevTools.Hardware {
	public class MccDAQBoardChannelFactory : IInputChannelFactory, IOutputChannelFactory {
		private readonly ILogger _logger;
		private readonly IScheduler _scheduler;

		public MccDAQBoardChannelFactory(ILogger logger = null, IScheduler scheduler = null) {
			_logger = logger;
			_scheduler = scheduler;
		}

		public IInputChannel CreateInputChannel(PulseOutputChannel pulseChannel) => _inputChannels[pulseChannel];

		public IOutputChannel CreateOutputChannel(OutputChannelType channelType) =>
			new MccDataAcqBoard(0, 0, 0);

		private Dictionary<PulseOutputChannel, MccDataAcqBoard> _inputChannels = new Dictionary<PulseOutputChannel, MccDataAcqBoard> {
			{ PulseOutputChannel.Channel_A, new MccDataAcqBoard(0, DigitalPortType.FirstPortA, 0) },
			{ PulseOutputChannel.Channel_B, new MccDataAcqBoard(0, DigitalPortType.FirstPortB, 1)  }
		};
	}

	/// <summary>
	/// Defines the <see cref="MccDataAcqBoard" />
	/// </summary>
	public class MccDataAcqBoard : IDisposable, IInputChannel, IOutputChannel {
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

		///// <summary>
		///// Defines the _log
		///// </summary>
		private readonly ILogger _log;

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
		/// Initializes a new instance of the <see cref="MccDataAcqBoard"/> class.
		/// </summary>
		/// <param name="boardNumber">The boardNumber<see cref="int"/></param>
		/// <param name="channelType">The channelType<see cref="DigitalPortType"/></param>
		/// <param name="channelNumber">The channelNumber<see cref="int"/></param>
		public MccDataAcqBoard(int boardNumber, DigitalPortType channelType, int channelNumber, ILogger logger = null) {
			_log = logger ?? ProverLogging.CreateLogger("MccDAQChannel");
			_ulStatErrorInfo = MccService.ErrHandling(ErrorReporting.PrintAll, ErrorHandling.DontStop);

			string boardName = string.Empty;
			var errorInfo = MccService.GetBoardName(boardNumber, ref boardName);
			if (errorInfo.Value == ErrorInfo.ErrorCode.BadBoard)
				throw new Exception("Board not found");

			_board = new MccBoard(boardNumber);

			_channelType = channelType;
			_channelNum = channelNumber;

			_log.LogInformation("Initialized DataAcqBoard: {0}, channel type {1}, channel number {2}", boardNumber, channelType, channelNumber);

			//var boardStatus = _board.GetStatus(out var status, out var curCount, out var curIndex, FunctionType.DaqoFunction);
			//if (boardStatus.Value != ErrorInfo.ErrorCode.NoErrors)
			//{
			//    throw new Exception("DAQ board could not be found or is not configured correctly.");
			//}
		}

		#endregion

		#region Enums

		private enum MotorValues {
			Start = 1023,
			Stop = 0
		}

		private enum OutputPorts {
			DaOut0 = 0,
			DaOut1 = 1
		}

		private enum SignalValues {
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

		public PulseOutputChannel Channel { get; }
		#endregion

		#region Methods

		/// <summary>
		/// The Dispose
		/// </summary>
		public void Dispose() {
			_board = null;
		}

		/// <summary>
		/// The StartMotor
		/// </summary>
		public void StartMotor() {
			Out(MotorValues.Start);
		}

		/// <summary>
		/// The StopMotor
		/// </summary>
		public void StopMotor() {
			Out(MotorValues.Stop);
		}

		/// <summary>
		/// The Out
		/// </summary>
		/// <param name="outputValue">The outputValue<see cref="MotorValues"/></param>
		private void Out(MotorValues outputValue) {

		}

		public int GetValue() {
			_ulStatErrorInfo = _board.DIn(_channelType, out short value);

			if (_ulStatErrorInfo.Value == ErrorInfo.ErrorCode.NoErrors) {
				if (value != 255) {
					if (_pulseIsCleared) {
						//_log.Trace($"Pulse value read -> value = {value}");
						_pulseIsCleared = false;
						return 1;
					}
				}
				else {
					_pulseIsCleared = true;
				}
			}
			else {
				if (_ulStatErrorInfo.Value != ErrorInfo.ErrorCode.BadBoard)
					_log.LogWarning("DAQ Input error: {0} - {1}", _ulStatErrorInfo.Message, _ulStatErrorInfo.Value);
			}
			return 0;
		}

		public void OutputValue(short value) {
			_ulStatErrorInfo = _board?.AOut(_channelNum, MccDaq.Range.UniPt05Volts, value);

			if (_ulStatErrorInfo?.Value != ErrorInfo.ErrorCode.NoErrors && _ulStatErrorInfo?.Value != ErrorInfo.ErrorCode.BadBoard)
				_log.LogWarning("DAQ Output error: {0}", _ulStatErrorInfo?.Message);
		}

		public void SignalStart() {
			OutputValue((short)MotorValues.Start);
		}

		public void SignalStop() {
			OutputValue((short)MotorValues.Stop);
		}

		#endregion
	}
}


//public IObservable<int> GetSimulator()
//{
//    _currentValue = 255;
//    return Observable.Create<int>(obs =>
//    {
//        var interval = Observable.Interval(TimeSpan.FromSeconds(PulseIntervalSeconds))
//            .Do(t => Debug.WriteLine($"Interval - {t} ms"))
//            .Select(_ => OffValue - _currentValue)
//            .Publish();

//        var onOff = interval
//            .Where(x => x != OffValue)
//            .Delay(TimeSpan.FromMilliseconds(62.5))
//            .Select(x => OffValue);

//        var cleanup2 = interval.Merge(onOff)
//            .Do(x => Debug.WriteLine($"Value = {x}"))
//            .Do(newValue => _currentValue = newValue)
//            .Subscribe(obs.OnNext);

//        return new CompositeDisposable(cleanup2, interval.Connect());
//    });
//}