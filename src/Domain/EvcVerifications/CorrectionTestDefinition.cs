namespace Domain.EvcVerifications
{
    public class CorrectionTestDefinition
    {
        #region Public Properties

        public bool IsVolumeTest { get; set; } = false;

        public int Level { get; set; }

        //public List<MechanicalUncorrectedTestLimit> MechanicalDriveTestLimits { get; set; } = new List<MechanicalUncorrectedTestLimit>();

        public decimal PressureGaugePercent { get; set; }

        public decimal TemperatureGauge { get; set; }

        #endregion

        
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