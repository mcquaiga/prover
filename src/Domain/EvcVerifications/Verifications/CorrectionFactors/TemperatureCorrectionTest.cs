using Devices.Core.Items.ItemGroups;

namespace Domain.EvcVerifications.Verifications.CorrectionFactors
{
    public class TemperatureCorrectionTest : VerificationTestEntity<TemperatureItems>
    {
        #region Public Properties
        protected  TemperatureCorrectionTest() { }
        public TemperatureCorrectionTest(TemperatureItems items, decimal expectedValue, decimal actualValue, decimal percentError, decimal gauge) 
            : base(items, expectedValue, actualValue, percentError)
        {
            Gauge = gauge;
        }

        public decimal Gauge { get; private set; }

        #endregion
    }
}