using MccDaq;
using Microsoft.Extensions.Logging;

namespace Prover.Application.ExternalDevices.DInOutBoards
{
    public class InputChannelDataAcqBoard : DataAcqBoardBase, IInputChannel
    {
        private const int OffSignalValue = 255;

        public InputChannelDataAcqBoard(ILogger logger, int boardNumber, DigitalPortType channelType,
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
                //if (value != OffSignalValue)
                //{
                //    if (_pulseIsCleared)
                //    {
                //        Log.LogTrace($"Pulse value read -> value = {value}");
                //        _pulseIsCleared = false;
                //        return 1;
                //    }
                //}
                //else
                //{
                //    _pulseIsCleared = true;
                //}
            }

            if (UlStatErrorInfo.Value != ErrorInfo.ErrorCode.BadBoard)
                Log.LogWarning("DAQ Input error: {0} - {1}", UlStatErrorInfo.Message, UlStatErrorInfo.Value);

            return OffSignalValue;
        }
    }
}