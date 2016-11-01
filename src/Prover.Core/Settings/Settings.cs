using System.Collections.Generic;

namespace Prover.Core.Settings
{
    public class Settings
    {
        public string LastInstrumentTypeUsed { get; set; }
        public string InstrumentCommPort { get; set; }
        public int InstrumentBaudRate { get; set; }
        public string TachCommPort { get; set; }
        public string ExportServiceAddress { get; set; }

        public List<GaugeDefaults> TemperatureGaugeDefaults { get; set; }
        public List<GaugeDefaults> PressureGaugeDefaults { get; set; }

        public void SetDefaults()
        {
            ExportServiceAddress = "";
            TemperatureGaugeDefaults = new List<GaugeDefaults>
            {
                new GaugeDefaults {Level = 0, Value = 32.0m},
                new GaugeDefaults {Level = 1, Value = 60.0m},
                new GaugeDefaults {Level = 2, Value = 90.0m}
            };

            PressureGaugeDefaults = new List<GaugeDefaults>
            {
                new GaugeDefaults {Level = 0, Value = 80.0m},
                new GaugeDefaults {Level = 1, Value = 50.0m},
                new GaugeDefaults {Level = 2, Value = 20.0m}
            };
        }
    }

    public class GaugeDefaults
    {
        public int Level { get; set; }
        public decimal Value { get; set; }
    }
}