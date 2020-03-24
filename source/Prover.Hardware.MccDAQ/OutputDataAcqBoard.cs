using System;
using MccDaq;
using Microsoft.Extensions.Logging;
using Prover.Shared.Interfaces;

namespace Prover.Hardware.MccDAQ
{
    public class OutputChannelDataAcqBoard : DataAcqBoardBase, IOutputChannel, IDisposable
    {
        public OutputChannelDataAcqBoard(ILogger logger, MccBoard boardNumber, int channelNumber) 
            : base(logger, boardNumber, 0, channelNumber)
        {
        }

        public void OutputValue(short value)
        {
            Out(value);
        }

        public void SignalStart()
        {
            OutputValue((short) MotorValues.Start);
        }

        public void SignalStop()
        {
            OutputValue((short) MotorValues.Stop);
        }


        /// <summary>
        ///     The Out
        /// </summary>
        /// <param name="outputValue">The outputValue<see /></param>
        private void Out(short outputValue)
        {
            UlStatErrorInfo = Board?.AOut(ChannelNum, Range.UniPt05Volts, (short) outputValue);

            if (UlStatErrorInfo?.Value != ErrorInfo.ErrorCode.NoErrors &&
                UlStatErrorInfo?.Value != ErrorInfo.ErrorCode.BadBoard)
                Log.LogWarning("DAQ Output error: {0}", UlStatErrorInfo?.Message);
        }

        #region Nested type: MotorValues

        private enum MotorValues
        {
            Start = 1023,
            Stop = 0
        }

        #endregion

        #region Nested type: OutputPorts

        private enum OutputPorts
        {
            DaOut0 = 0,
            DaOut1 = 1
        }

        #endregion
    }
}