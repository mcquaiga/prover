using System.Collections.Generic;

namespace Domain.Models.EvcVerifications
{
    public class CorrectionTestDefinition
    {
        public bool IsVolumeTest { get; set; } = false;

        public int Level { get; set; }

        public List<MechanicalUncorrectedTestLimit> MechanicalDriveTestLimits { get; set; } = new List<MechanicalUncorrectedTestLimit>();

        public decimal PressureGaugePercent { get; set; }

        public decimal TemperatureGauge { get; set; }

        public class MechanicalUncorrectedTestLimit
        {
            /// <summary>
            /// Gets or sets the CuFtValue
            /// </summary>
            public decimal CuFtValue { get; set; }

            /// <summary>
            /// Gets or sets the UncorrectedPulses
            /// </summary>
            public int UncorrectedPulses { get; set; }
        }
    }
}