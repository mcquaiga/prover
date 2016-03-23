using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private MccBoard _board;
        private readonly DigitalPortType _channelType;
        private readonly int _channelNum;
        private ErrorInfo _ulStatErrorInfo;
        private bool _pulseIsCleared;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public DataAcqBoard(int boardNumber, DigitalPortType channelType, int channelNumber)
        {
            _ulStatErrorInfo = MccService.ErrHandling(ErrorReporting.PrintAll, ErrorHandling.DontStop);

            _board = new MccBoard(boardNumber);
            _channelType = channelType;
            _channelNum = channelNumber;

            _pulseIsCleared = true;
            _log.Info("Initialized DataAcqBoard: {0}, channel type {1}, channel number {2}", boardNumber, channelType, channelNumber);
        }

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
            short value = 0;

            _ulStatErrorInfo = _board.DIn(_channelType, out value);

            if (_ulStatErrorInfo.Value == ErrorInfo.ErrorCode.NoErrors)
            {
                if (value != 255)
                {
                    if (_pulseIsCleared)
                    {
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
                _log.Warn("DAQ Input error: {0}", _ulStatErrorInfo.Message);
            }
            return 0;           
        }

        private void Out(MotorValues outputValue)
        {
            _board.AOut(_channelNum, Range.UniPt05Volts, (short)outputValue);
            
        }

        public void Dispose()
        {
            _board = null;
            GC.SuppressFinalize(this);
        }
    }
}
