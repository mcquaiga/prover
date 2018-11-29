﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MccDaq;
using NLog;

namespace Prover.Core.ExternalDevices.DInOutBoards
{
    public class DataAcqBoard : IDisposable, IDInOutBoard
    {
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

        private const decimal PulseTimingDefaultSeconds = 0.0625m;
        private readonly int _channelNum;
        private readonly DigitalPortType _channelType;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private MccBoard _board;
        private bool _pulseIsCleared;
        private bool _isPulseWaitTimeOver;
        private ErrorInfo _ulStatErrorInfo;

        public DataAcqBoard(int boardNumber, DigitalPortType channelType, int channelNumber)
        {
            _ulStatErrorInfo = MccService.ErrHandling(ErrorReporting.PrintAll, ErrorHandling.DontStop);

            _board = new MccBoard(boardNumber);
            _channelType = channelType;
            _channelNum = channelNumber;

            _isPulseWaitTimeOver = true;
            _log.Info("Initialized DataAcqBoard: {0}, channel type {1}, channel number {2}", boardNumber, channelType,
                channelNumber);

        }

        public decimal PulseTiming { get; set; } = PulseTimingDefaultSeconds;

        public void StartMotor()
        {
            Out(MotorValues.Start);
        }

        public void StopMotor()
        {
            Out(MotorValues.Stop);
        }

        public int ReadInput()
        {
             var boardStatus = _board.GetStatus(out var status, out var curCount, out var curIndex, FunctionType.AiFunction);
            if (boardStatus.Value != ErrorInfo.ErrorCode.NoErrors)
            {
                throw new Exception("DAQ board could not be found or is not configured correctly.");
            }

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

        public void Dispose()
        {
            _board = null;
            GC.SuppressFinalize(this);
        }

        public short InputValue { get; private set; }

        private void Out(MotorValues outputValue)
        {
            _ulStatErrorInfo = _board?.AOut(_channelNum, Range.UniPt05Volts, (short) outputValue);
            if (_ulStatErrorInfo != null && (_ulStatErrorInfo.Value != ErrorInfo.ErrorCode.NoErrors) &&
                (_ulStatErrorInfo.Value != ErrorInfo.ErrorCode.BadBoard))
                _log.Warn("DAQ Output error: {0}", _ulStatErrorInfo.Message);
        }        
    }
}