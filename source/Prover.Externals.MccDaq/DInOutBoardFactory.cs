using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MccDaq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Shared;
using Prover.Shared.Hardware;
using Prover.Shared.Interfaces;

namespace Prover.Externals.MccDaq
{

    public class DaqBoardChannelFactory : IInputChannelFactory, IOutputChannelFactory
    {
        private MccBoard Board;
        private const string InstaCalPath = "C:\\Program Files (x86)\\Measurement Computing\\DAQ\\inscal32.exe";


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
            _inputLogger = logFactory?.CreateLogger<InputChannelDataAcqBoard>() ?? NullLogger<InputChannelDataAcqBoard>.Instance;
            _outputLogger = logFactory?.CreateLogger<OutputChannelDataAcqBoard>() ?? NullLogger<OutputChannelDataAcqBoard>.Instance;

            Board = FindBoard();
        }

        //int boardNumber, DigitalPortType channelType, int channelNumber
        public IInputChannel CreateInputChannel(PulseOutputChannel pulseChannel)
        {
            if (Board != null)
            {
                try
                {

                    var channelType = _portTypeMappings[pulseChannel];
                    var channelNumber = _channelNumberMappings[pulseChannel];

                    return new InputChannelDataAcqBoard(_inputLogger, Board, channelType, channelNumber);
                }
                catch (Exception ex)
                {
                    _inputLogger.LogError(ex, "An error occured creating input channel.");
                }
            }

            return new EmptyDInOutBoard(_inputLogger);
        }
        
        public IOutputChannel CreateOutputChannel(OutputChannelType channelType)
        {
            var channelNumber = (int)channelType;
            if (Board != null)
            {
                try
                {
                    var board = new OutputChannelDataAcqBoard(_outputLogger, Board, channelNumber);

                    return board;
                }
                catch (Exception ex)
                {
                    _outputLogger.LogError(ex,  "An error occured creating input channel.");
                }
            }

            return new EmptyDInOutBoard(_outputLogger);
        }

        private MccBoard FindBoard()
        {
            var boardFound = false;
            MccBoard board = null;
            int boardNumber;

            for (boardNumber = 0; boardNumber < 99; boardNumber++)
            {
                board = new MccBoard(boardNumber);
                if (board.BoardName.Contains("1208LS"))
                {
                    boardFound = true;
                    break;
                }
            }

            if (boardFound == false)
            {
                _inputLogger.LogWarning("USB-1208LS not found. Attempting to start InstaCal.");
                if (File.Exists(InstaCalPath))
                    Process.Start(InstaCalPath);
            }

            return board;
        }
    }
}