using System;
using System.Collections.Generic;
using MccDaq;
using Microsoft.Extensions.Logging;
using Prover.Shared;

namespace Prover.Application.ExternalDevices.DInOutBoards
{
    public interface IInputChannelFactory
    {
        IInputChannel CreateInputChannel(PulseOutputChannel pulseChannel);
    }

    public interface IOutputChannelFactory
    {
        
        IOutputChannel CreateOutputChannel(OutputChannelType channelType);
    }

    public class DaqBoardChannelFactory : IInputChannelFactory, IOutputChannelFactory
    {
        private int BoardNumber = 0;

        private readonly Dictionary<PulseOutputChannel, int> _channelNumberMappings =
            new Dictionary<PulseOutputChannel, int>
            {
                {PulseOutputChannel.Channel_A, 0},
                {PulseOutputChannel.Channel_B, 1}
            };


        private readonly ILogger<InputChannelDataAcqBoard> _inputLogger;
        private readonly ILogger<OutputChannelDataAcqBoard> _outputLogger;

        private readonly Dictionary<PulseOutputChannel, DigitalPortType> _portTypeMappings =
            new Dictionary<PulseOutputChannel, DigitalPortType>
            {
                {PulseOutputChannel.Channel_A, DigitalPortType.FirstPortA},
                {PulseOutputChannel.Channel_B, DigitalPortType.FirstPortB}
            };

        public DaqBoardChannelFactory(ILoggerFactory logFactory)
        {
            _inputLogger = logFactory.CreateLogger<InputChannelDataAcqBoard>();
            _outputLogger = logFactory.CreateLogger<OutputChannelDataAcqBoard>();

            FindBoardNumber();
        }

        //int boardNumber, DigitalPortType channelType, int channelNumber
        public IInputChannel CreateInputChannel(PulseOutputChannel pulseChannel)
        {
            try
            {
                var channelType = _portTypeMappings[pulseChannel];
                var channelNumber = _channelNumberMappings[pulseChannel];

                var board = new InputChannelDataAcqBoard(_inputLogger, BoardNumber, channelType, channelNumber);

                return board;
            }
            catch (Exception)
            {
                return new EmptyDInOutBoard(_inputLogger);
            }
        }
        
        public IOutputChannel CreateOutputChannel(OutputChannelType channelType)
        {
            var channelNumber = (int)channelType;

            try
            {
                var board = new OutputChannelDataAcqBoard(_outputLogger, BoardNumber, channelNumber);

                return board;
            }
            catch (Exception)
            {
                return new EmptyDInOutBoard(_outputLogger);
            }
        }

        private int FindBoardNumber()
        {
            var boardFound = false;

            for (BoardNumber = 0; BoardNumber < 99; BoardNumber++)
            {
                var board = new MccBoard(BoardNumber);
                if (board.BoardName.Contains("1208LS"))
                {
                    boardFound = true;
                    board.FlashLED();
                    break;
                }
            }

            if (boardFound == false)
            {
                _inputLogger.LogWarning("USB-1208LS not found  Please run InstaCal.");
                return -1;
            }

            return BoardNumber;
        }
    }
}