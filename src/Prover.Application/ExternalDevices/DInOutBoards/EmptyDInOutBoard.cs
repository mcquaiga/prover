using Microsoft.Extensions.Logging;

namespace Prover.Application.ExternalDevices.DInOutBoards
{
    public class EmptyDInOutBoard : IInputChannel, IOutputChannel
    {
        private readonly ILogger _logger;

        public EmptyDInOutBoard(ILogger logger)
        {
            _logger = logger;
        }

        public decimal PulseTiming { get; set; }

        public int GetValue()
        {
            return 0;
        }

       
        public void OutputValue(short value)
        {
            _logger.LogDebug($"EmptyInOutBoard - Output value {value}");
        }

        public void SignalStart()
        {
            throw new System.NotImplementedException();
        }

        public void SignalStop()
        {
            throw new System.NotImplementedException();
        }
    }

    public class RandomValueInputChannel : IInputChannel
    {
        private readonly ILogger _logger;

        public RandomValueInputChannel(ILogger logger)
        {
            _logger = logger;
        }

        public decimal PulseTiming { get; set; }

        public int GetValue()
        {
            return 0;
        }
    }
}