using MccDaq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.ExternalDevices.DInOutBoards
{
    public static class DInOutBoardFactory
    {
        public static IDInOutBoard CreateBoard(int boardNumber, DigitalPortType channelType, int channelNumber)
        {
            var board = new DataAcqBoard(boardNumber, channelType, channelNumber);
            
            return board;
        }
    }
}
