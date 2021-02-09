using MccDaq;
using Microsoft.Extensions.Logging;
using Prover.Shared.Interfaces;

namespace Prover.Externals.MccDaq {
	public class InputChannelDataAcqBoard : DataAcqBoardBase, IInputChannel {
		private const int OffSignalValue = 255;

		public InputChannelDataAcqBoard(ILogger logger, MccBoard boardNumber, DigitalPortType channelType,
			int channelNumber)
			: base(logger, boardNumber, channelType, channelNumber) {
		}

		public int GetValue() {
			UlStatErrorInfo = Board.DIn(ChannelType, out short value);

			if (UlStatErrorInfo.Value == ErrorInfo.ErrorCode.NoErrors) {
				Log.LogTrace("DAQ Input: P: {0} - C: {1} - Value: {2}", ChannelType, ChannelNum, value);
				return value;
			}

			if (UlStatErrorInfo.Value != ErrorInfo.ErrorCode.BadBoard)
				Log.LogWarning("DAQ Input error: {0} - {1}", UlStatErrorInfo.Message, UlStatErrorInfo.Value);

			return OffSignalValue;
		}
	}
}