using System.Collections.Generic;

namespace Prover.Core.Settings
{
    public class GaugeDefaults
    {
        public int Level { get; set; }
        public decimal Value { get; set; }
    }

    public class MechanicalUncorrectedTestLimit
    {
        public decimal CuFtValue { get; set; }
        public int UncorrectedPulses { get; set; }
    }

    public class CertificateSettings
    {
        public string McRegistrationNumber { get; set; } = string.Empty;
        public string MeasurementApparatus { get; set; } = string.Empty;
    }

    public class TestSettings
    {
        public enum VolumeTestType
        {
            Automatic,
            Manual
        }
       
        public bool StabilizeLiveReadings { get; set; }
        public VolumeTestType MechanicalDriveVolumeTestType { get; set; }
        public List<MechanicalUncorrectedTestLimit> MechanicalUncorrectedTestLimits { get; set; }
        public List<GaugeDefaults> TemperatureGaugeDefaults { get; set; }
        public bool UpdateAbsolutePressure { get; set; } = true;
        public List<GaugeDefaults> PressureGaugeDefaults { get; set; }
        public bool RunVolumeSyncTest { get; set; }

        public static TestSettings CreateDefault()
        {
            return new TestSettings()
            {
                StabilizeLiveReadings = false,
                RunVolumeSyncTest = false,
                MechanicalDriveVolumeTestType = TestSettings.VolumeTestType.Automatic,
                TemperatureGaugeDefaults = new List<GaugeDefaults>
                {
                    new GaugeDefaults {Level = 0, Value = 32.0m},
                    new GaugeDefaults {Level = 1, Value = 60.0m},
                    new GaugeDefaults {Level = 2, Value = 90.0m}
                },
                PressureGaugeDefaults = new List<GaugeDefaults>
                {
                    new GaugeDefaults {Level = 0, Value = 80.0m},
                    new GaugeDefaults {Level = 1, Value = 50.0m},
                    new GaugeDefaults {Level = 2, Value = 20.0m}
                },

                MechanicalUncorrectedTestLimits = new List<MechanicalUncorrectedTestLimit>
                {
                    new MechanicalUncorrectedTestLimit {CuFtValue = 1, UncorrectedPulses = 1000},
                    new MechanicalUncorrectedTestLimit {CuFtValue = 10, UncorrectedPulses = 100},
                    new MechanicalUncorrectedTestLimit {CuFtValue = 100, UncorrectedPulses = 10},
                    new MechanicalUncorrectedTestLimit {CuFtValue = 1000, UncorrectedPulses = 1}
                }
            };
        }       
    }

    public class LocalSettings
    {
        public string TachCommPort { get; set; }
        public bool TachIsNotUsed { get; set; }

        public string LastClientSelected { get; set; }

        public string LastInstrumentTypeUsed { get; set; }
        public string InstrumentCommPort { get; set; }
        public int InstrumentBaudRate { get; set; }
        public bool InstrumentUseIrDaPort { get; set; }
        public double WindowHeight { get; set; } = 800;
        public double WindowWidth { get; set; } = 800;
        public string WindowState { get; set; } = "Normal";
    }

    public class SharedSettings
    {
        public CertificateSettings CertificateSettings { get; set; }
       
        public TestSettings TestSettings { get; set; }

        public static SharedSettings Create()
        {
            return new SharedSettings()
            {
                CertificateSettings = new CertificateSettings(),
                TestSettings = TestSettings.CreateDefault()
            };
        }
    }
}