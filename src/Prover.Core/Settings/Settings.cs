using Prover.SerialProtocol;

namespace Prover.Core.Settings
{
    public class Settings
    {
        public string LastInstrumentTypeUsed { get; set; }
        public string InstrumentCommPort { get; set; }
        public BaudRateEnum InstrumentBaudRate { get; set; }
        public string TachCommPort { get; set; }
    }
}
