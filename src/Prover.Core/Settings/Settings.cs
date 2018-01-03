using System.Collections.Generic;

namespace Prover.Core.Settings
{
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

    public class TestSettings
    {
        public enum VolumeTestType
        {
            Automatic,
            Manual
        }

        public List<TestPointSetting> TestPoints { get; set; }
        public bool StabilizeLiveReadings { get; set; }
        public VolumeTestType MechanicalDriveVolumeTestType { get; set; }
        public List<MechanicalUncorrectedTestLimit> MechanicalUncorrectedTestLimits { get; set; }
        public bool UpdateAbsolutePressure { get; set; } = true;
        public bool RunVolumeSyncTest { get; set; }

        public static TestSettings CreateDefault()
        {
            return new TestSettings()
            {
                StabilizeLiveReadings = false,
                RunVolumeSyncTest = false,
                MechanicalDriveVolumeTestType = VolumeTestType.Automatic,

                MechanicalUncorrectedTestLimits = new List<MechanicalUncorrectedTestLimit>
                {
                    new MechanicalUncorrectedTestLimit {CuFtValue = 1, UncorrectedPulses = 100},
                    new MechanicalUncorrectedTestLimit {CuFtValue = 10, UncorrectedPulses = 10},
                    new MechanicalUncorrectedTestLimit {CuFtValue = 100, UncorrectedPulses = 10},
                    new MechanicalUncorrectedTestLimit {CuFtValue = 1000, UncorrectedPulses = 1}
                },

                TestPoints = new List<TestPointSetting>()
                {
                    new TestPointSetting()
                    {
                        Level = 0,
                        PressureGaugePercent = 80.0m,
                        TemperatureGauge = 32,
                        IsVolumeTest = true
                    },
                    new TestPointSetting()
                    {
                        Level = 1,
                        PressureGaugePercent = 50.0m,
                        TemperatureGauge = 60,
                        IsVolumeTest = false
                    },
                    new TestPointSetting()
                    {
                        Level = 2,
                        PressureGaugePercent = 20.0m,
                        TemperatureGauge = 90,
                        IsVolumeTest = false
                    },
                }
            };
        }

        public class TestPointSetting
        {
            public int Level { get; set; }
            public bool IsVolumeTest { get; set; } = false;
            public decimal PressureGaugePercent { get; set; }
            public decimal TemperatureGauge { get; set; }
        }

        public class MechanicalUncorrectedTestLimit
        {
            public decimal CuFtValue { get; set; }
            public int UncorrectedPulses { get; set; }
        }
    }

    public class CertificateSettings
    {
        public string McRegistrationNumber { get; set; } = string.Empty;
        public List<MeasurementApparatus> MeasurementApparatuses { get; set; } = new List<MeasurementApparatus>();

        public class MeasurementApparatus
        {            
            public string Description { get; set; }
            public string SerialNumbers { get; set; }
        }
    }
}