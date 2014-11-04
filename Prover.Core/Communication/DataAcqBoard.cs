using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MccDaq;

namespace Prover.Core.Communication
{
    public enum MotorValues
    {
        Start = 1023,
        Stop = 0
    }

    public enum OutputPorts
    {
        DaOut0 = 0,
        DaOut1 = 1
    }

    public class DataAcqBoard : IDisposable
    {
        private MccBoard _board;
        private readonly DigitalPortType _channelType;
        private readonly int _channelNum;
        private ErrorInfo _ulStatErrorInfo;
        private bool _pulseIsCleared;

        public DataAcqBoard(int boardNumber, DigitalPortType channelType, int channelNumber)
        {
            _board = new MccBoard(boardNumber);
            _channelType = channelType;
            _channelNum = channelNumber;
            _ulStatErrorInfo = MccService.ErrHandling(ErrorReporting.PrintAll, ErrorHandling.StopAll);
            _pulseIsCleared = true;
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
            return 0;
        }

        private void Out(MotorValues outputValue)
        {
            _board.AOut(_channelNum, Range.UniPt05Volts, (short)outputValue);
            
        }

        private void In(int value)
        {
            
        }


        public void Dispose()
        {
            if (_board == null) return;
            _board.DeviceLogout();
            _board = null;
        }
    }
}
