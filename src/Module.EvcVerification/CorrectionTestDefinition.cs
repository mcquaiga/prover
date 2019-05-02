using System.Collections.Generic;

namespace Module.EvcVerification
{
    public class CorrectionTestDefinition
    {
        #region Properties

        public bool IsVolumeTest { get; set; } = false;

        public int Level { get; set; }

        public List<MechanicalUncorrectedTestLimit> MechanicalDriveTestLimits { get; set; } = new List<MechanicalUncorrectedTestLimit>();

        public decimal PressureGaugePercent { get; set; }

        public decimal TemperatureGauge { get; set; }

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

        #endregion
    }
}