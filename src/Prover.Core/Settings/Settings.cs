using Prover.SerialProtocol;

namespace Prover.Core.Settings
{
    public interface ISettings
    {
        string InstrumentCommPort { get; set;}
        BaudRateEnum InstrumentBaudRate { get; set; }
        string TachCommPort { get; set; }
    }

    public class Settings
    {
        public Settings()
        {
            InstrumentCommPort = string.Empty;
            InstrumentBaudRate = BaudRateEnum.b38400;
            TachCommPort = string.Empty;
        }

        public string InstrumentCommPort { get; set; }
        public BaudRateEnum InstrumentBaudRate { get; set; }
        public string TachCommPort { get; set; }
    }
}
