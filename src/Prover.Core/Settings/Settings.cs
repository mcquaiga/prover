using System.Collections.Generic;

namespace Prover.Core.Settings
{
    public class Settings
    {
        public string LastInstrumentTypeUsed { get; set; }
        public string LastDriveTypeUsed { get; set; }
        public string InstrumentCommPort { get; set; }
        public int InstrumentBaudRate { get; set; }
        public string TachCommPort { get; set; }
        public string ExportServiceAddress { get; set; }
        public bool UpdateAbsolutePressure { get; set; } = true;

        public List<GaugeDefaults> TemperatureGaugeDefaults { get; set; }
        public List<GaugeDefaults> PressureGaugeDefaults { get; set; }
        public List<MechanicalUncorrectedTestLimit> MechanicalUncorrectedTestLimits { get; set; }        
        public Dictionary<int, string> TocResetItems { get; set; }

        public void SetDefaults()
        {
            ExportServiceAddress = "";

            if (TemperatureGaugeDefaults == null)
                TemperatureGaugeDefaults = new List<GaugeDefaults>
                {
                    new GaugeDefaults {Level = 0, Value = 32.0m},
                    new GaugeDefaults {Level = 1, Value = 60.0m},
                    new GaugeDefaults {Level = 2, Value = 90.0m}
                };

            if (PressureGaugeDefaults == null)
                PressureGaugeDefaults = new List<GaugeDefaults>
                {
                    new GaugeDefaults {Level = 0, Value = 80.0m},
                    new GaugeDefaults {Level = 1, Value = 50.0m},
                    new GaugeDefaults {Level = 2, Value = 20.0m}
                };

            if (MechanicalUncorrectedTestLimits == null)
                MechanicalUncorrectedTestLimits = new List<MechanicalUncorrectedTestLimit>
                {
                    new MechanicalUncorrectedTestLimit { CuFtValue = 1, UncorrectedPulses = 1000},
                    new MechanicalUncorrectedTestLimit { CuFtValue = 10, UncorrectedPulses = 100},
                    new MechanicalUncorrectedTestLimit { CuFtValue = 100, UncorrectedPulses = 10},
                    new MechanicalUncorrectedTestLimit { CuFtValue = 1000, UncorrectedPulses = 1}  
                };

            if (TocResetItems == null)
            {
                TocResetItems = new Dictionary<int, string>()
                {
                    { 859, "0" },
                    { 860, "0" },
                    { 850, "0" },
                    { 851, "0" },
                    { 852, "0" }          
                };
            }
        }
    }

    public class GaugeDefaults
    {
        public int Level { get; set; }
        public decimal Value { get; set; }
    }

    public class MechanicalUncorrectedTestLimit
    {
        public int CuFtValue { get; set; }
        public int UncorrectedPulses { get; set; }
    }
}