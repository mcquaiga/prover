namespace Prover.Core.Settings
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="TestSettings" />
    /// </summary>
    public class TestSettings
    {
        #region Enums

        public enum VolumeTestType
        {
            Automatic,
            Manual
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the CorrectedErrorThreshold
        /// </summary>
        public decimal CorrectedErrorThreshold { get; set; } = 1.5m;

        /// <summary>
        /// Gets or sets the MechanicalDriveVolumeTestType
        /// </summary>
        public VolumeTestType MechanicalDriveVolumeTestType { get; set; }

        /// <summary>
        /// Gets or sets the MechanicalUncorrectedTestLimits
        /// </summary>
        public List<MechanicalUncorrectedTestLimit> MechanicalUncorrectedTestLimits { get; set; }

        /// <summary>
        /// Gets or sets the MeterDisplacementErrorThreshold
        /// </summary>
        public decimal MeterDisplacementErrorThreshold { get; set; } = 1.0m;

        /// <summary>
        /// Gets or sets a value indicating whether RunVolumeSyncTest
        /// </summary>
        public bool RunVolumeSyncTest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether StabilizeLiveReadings
        /// </summary>
        public bool StabilizeLiveReadings { get; set; }

        /// <summary>
        /// Gets or sets the TestPoints
        /// </summary>
        public List<TestPointSetting> TestPoints { get; set; }

        /// <summary>
        /// Gets or sets the TocResetItems
        /// </summary>
        public Dictionary<int, string> TocResetItems { get; set; }

        /// <summary>
        /// Gets or sets the UncorrectedErrorThreshold
        /// </summary>
        public decimal UncorrectedErrorThreshold { get; set; } = 0.1m;

        /// <summary>
        /// Gets or sets a value indicating whether UpdateAbsolutePressure
        /// </summary>
        public bool UpdateAbsolutePressure { get; set; } = true;

        #endregion

        #region Methods

        /// <summary>
        /// The CreateDefault
        /// </summary>
        /// <returns>The <see cref="TestSettings"/></returns>
        public static TestSettings CreateDefault() => new TestSettings()
        {
            CorrectedErrorThreshold = 1.5m,
            UncorrectedErrorThreshold = 0.1m,
            MeterDisplacementErrorThreshold = 1.0m,

            StabilizeLiveReadings = true,
            RunVolumeSyncTest = true,
            MechanicalDriveVolumeTestType = VolumeTestType.Automatic,

            MechanicalUncorrectedTestLimits = new List<MechanicalUncorrectedTestLimit>
            {
                new MechanicalUncorrectedTestLimit { CuFtValue = 1, UncorrectedPulses = 100 },
                new MechanicalUncorrectedTestLimit { CuFtValue = 10, UncorrectedPulses = 10 },
                new MechanicalUncorrectedTestLimit { CuFtValue = 100, UncorrectedPulses = 10 },
                new MechanicalUncorrectedTestLimit { CuFtValue = 1000, UncorrectedPulses = 1 }
            },

            TocResetItems = new Dictionary<int, string>
                {
                    { 859, "0" },
                    { 860, "0" }
                },

            TestPoints = new List<TestPointSetting>
                {
                    new TestPointSetting
                    {
                        Level = 0,
                        PressureGaugePercent = 80.0m,
                        TemperatureGauge = 32,
                        IsVolumeTest = true
                    },
                    new TestPointSetting
                    {
                        Level = 1,
                        PressureGaugePercent = 50.0m,
                        TemperatureGauge = 60,
                        IsVolumeTest = false
                    },
                    new TestPointSetting
                    {
                        Level = 2,
                        PressureGaugePercent = 20.0m,
                        TemperatureGauge = 90,
                        IsVolumeTest = false
                    },
                }
        };

        #endregion

        /// <summary>
        /// Defines the <see cref="MechanicalUncorrectedTestLimit" />
        /// </summary>
        public class MechanicalUncorrectedTestLimit
        {
            #region Properties

            /// <summary>
            /// Gets or sets the CuFtValue
            /// </summary>
            public decimal CuFtValue { get; set; }

            /// <summary>
            /// Gets or sets the UncorrectedPulses
            /// </summary>
            public int UncorrectedPulses { get; set; }

            #endregion
        }

        /// <summary>
        /// Defines the <see cref="TestPointSetting" />
        /// </summary>
        public class TestPointSetting
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether IsVolumeTest
            /// </summary>
            public bool IsVolumeTest { get; set; } = false;

            /// <summary>
            /// Gets or sets the Level
            /// </summary>
            public int Level { get; set; }

            /// <summary>
            /// Gets or sets the PressureGaugePercent
            /// </summary>
            public decimal PressureGaugePercent { get; set; }

            /// <summary>
            /// Gets or sets the TemperatureGauge
            /// </summary>
            public decimal TemperatureGauge { get; set; }

            #endregion
        }
    }
}
