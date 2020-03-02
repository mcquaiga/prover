using MccDaq;
using Microsoft.Extensions.Logging;
using Prover.Shared.Interfaces;

namespace Prover.Hardware.MccDAQ
{
    public class InputChannelDataAcqBoard : DataAcqBoardBase, IInputChannel
    {
        private const int OffSignalValue = 255;

        public InputChannelDataAcqBoard(ILogger logger, MccBoard boardNumber, DigitalPortType channelType,
            int channelNumber)
            : base(logger, boardNumber, channelType, channelNumber)
        {
        }

        public int GetValue()
        {
            UlStatErrorInfo = Board.DIn(ChannelType, out short value);

            if (UlStatErrorInfo.Value == ErrorInfo.ErrorCode.NoErrors)
            {
                return value;
            }

            if (UlStatErrorInfo.Value != ErrorInfo.ErrorCode.BadBoard)
                Log.LogWarning("DAQ Input error: {0} - {1}", UlStatErrorInfo.Message, UlStatErrorInfo.Value);

            return OffSignalValue;
        }
    }
}