using Devices.Core.Items.ItemGroups;

namespace Prover.Domain.EvcVerifications.Verifications.CorrectionFactors
{
    public class PressureCorrectionTest : VerificationTestEntity<PressureItems>
    {
        protected PressureCorrectionTest() {}

        #region Public Properties
        public PressureCorrectionTest(PressureItems items, decimal expectedValue, decimal actualValue, decimal percentError, decimal gauge, decimal atmGauge) 
            : base(items, expectedValue, actualValue, percentError)
        {
            Gauge = gauge;
            AtmosphericGauge = atmGauge;
        }

        public decimal Gauge { get; set; }
        public decimal AtmosphericGauge { get; set; }

        #endregion
    }
}