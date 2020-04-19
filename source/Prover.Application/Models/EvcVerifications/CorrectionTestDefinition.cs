using System.Collections.Generic;

namespace Prover.Application.Models.EvcVerifications
{
    public class VerificationTestOptions
    {
        public ICollection<CorrectionTestDefinition> CorrectionTestDefinitions { get; set; }

        public ICollection<VolumeTestDefinition> VolumeTestDefinitions { get; set; }

        public static VerificationTestOptions Defaults = new VerificationTestOptions()
        {
                CorrectionTestDefinitions = CorrectionTestDefinition.Defaults,
                VolumeTestDefinitions = new List<VolumeTestDefinition> {VolumeTestDefinition.Default}
        };
    }

    public class VolumeTestDefinition
    {
        public ICollection<VolumeInputTestSample> VolumeInputTargets { get; set; }
        public int Level { get; set; }

        public static VolumeTestDefinition Default = new VolumeTestDefinition(){ Level = 0, VolumeInputTargets = new List<VolumeInputTestSample>()};
    }

    public class CorrectionTestDefinition
    {
        #region Public Properties

        public bool IsVolumeTest { get; set; } = false;

        public int Level { get; set; }

        //public List<MechanicalUncorrectedTestLimit> MechanicalDriveTestLimits { get; set; } = new List<MechanicalUncorrectedTestLimit>();

        public decimal PressureGaugePercent { get; set; }

        public decimal TemperatureGauge { get; set; }

        #endregion

        public static ICollection<CorrectionTestDefinition> Defaults = new List<CorrectionTestDefinition>
        {
                new CorrectionTestDefinition
                {
                        Level = 0,
                        TemperatureGauge = 32,
                        PressureGaugePercent = 80,
                        IsVolumeTest = true
                },
                new CorrectionTestDefinition
                {
                        Level = 1,
                        TemperatureGauge = 60,
                        PressureGaugePercent = 50,
                        IsVolumeTest = false
                },
                new CorrectionTestDefinition
                {
                        Level = 2,
                        TemperatureGauge = 90,
                        PressureGaugePercent = 20,
                        IsVolumeTest = false
                }
        };
    }

    public class VolumeInputTestSample
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the CuFtValue
        /// </summary>
        public decimal CuFtValue { get; set; }

        /// <summary>
        ///     Gets or sets the UncorrectedPulses
        /// </summary>
        public int UncorrectedPulseTarget { get; set; }

        #endregion
    }
}