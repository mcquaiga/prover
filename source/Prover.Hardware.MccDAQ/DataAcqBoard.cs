using System;
using MccDaq;
using Microsoft.Extensions.Logging;

namespace Prover.Hardware.MccDAQ
{
    /// <summary>
    ///     Defines the <see cref="DataAcqBoardBase" />
    /// </summary>
    public abstract class DataAcqBoardBase : IDisposable
    {
        /// <summary>
        ///     Defines the PulseTimingDefaultSeconds
        /// </summary>
        private const decimal PulseTimingDefaultSeconds = 0.0625m;

        /// <summary>
        ///     Defines the _channelNum
        /// </summary>
        protected readonly int ChannelNum;

        /// <summary>
        ///     Defines the _channelType
        /// </summary>
        protected readonly DigitalPortType ChannelType;

        /// <summary>
        ///     Defines the _log
        /// </summary>
        protected readonly ILogger Log;

        /// <summary>
        ///     Defines the _board
        /// </summary>
        protected MccBoard Board;


        /// <summary>
        ///     Defines the _ulStatErrorInfo
        /// </summary>
        protected ErrorInfo UlStatErrorInfo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataAcqBoardBase" /> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="boardNumber">The boardNumber<see cref="int" /></param>
        /// <param name="channelType">The channelType<see cref="MccDaq.DigitalPortType" /></param>
        /// <param name="channelNumber">The channelNumber<see cref="int" /></param>
        public  DataAcqBoardBase(ILogger logger, MccBoard board, DigitalPortType channelType,
            int channelNumber)
        {
            Log = logger;

            Board = board ?? throw new Exception("DAQ board could not be found or is not configured correctly.");

            //UlStatErrorInfo = MccService.ErrHandling(ErrorReporting.PrintAll, ErrorHandling.DontStop);

            //if (boardNumber != null)
            //{
            //    Board = boardNumber;
            //    //var boardStatus = Board.GetStatus(out var status, out var curCount, out var curIndex, FunctionType.DaqoFunction);
            //    var boardStatus = Board.BoardConfig.GetDeviceSerialNum(out var serialNum);
            //    if (boardStatus.Value != ErrorInfo.ErrorCode.NoErrors)
            //    {
            //        throw new Exception("DAQ board could not be found or is not configured correctly.");
            //    }
            //}

            var boardName = Board.BoardName;
            ChannelType = channelType;
            ChannelNum = channelNumber;

            Log.LogDebug("Initialized DataAcqBoard: {0}, channel type {1}, channel number {2}", boardName,
                channelType, channelNumber);
        }


        /// <summary>
        ///     The Dispose
        /// </summary>
        public void Dispose()
        {
            Log.LogDebug(
                $"Disposing DAQ Board {Board.BoardName} - #{Board.BoardNum} - Channel {ChannelType} - Channel #{ChannelNum}");
            Board.DeviceLogout();

            Board = null;
        }
    }
}