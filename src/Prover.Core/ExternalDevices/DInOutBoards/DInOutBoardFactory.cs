using System;
using MccDaq;

namespace Prover.Core.ExternalDevices.DInOutBoards
{
    public static class DInOutBoardFactory
    {
        public static IDInOutBoard CreateBoard(int boardNumber, DigitalPortType channelType, int channelNumber)
        {
            try
            {
                var board = new DataAcqBoard(boardNumber, channelType, channelNumber);

                return board;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}