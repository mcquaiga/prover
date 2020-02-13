using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items.ItemGroups;

namespace Domain.EvcVerifications.Verifications.CorrectionFactors
{
    public class TemperatureCorrectionTest : VerificationTestEntity<TemperatureItems>
    {
        #region Public Properties
        protected  TemperatureCorrectionTest() { }
        public TemperatureCorrectionTest(TemperatureItems items, decimal expectedValue, decimal actualValue, decimal percentError, bool verified, decimal gauge) 
            : base(items, expectedValue, actualValue, percentError, verified)
        {
            Gauge = gauge;
        }

        public decimal Gauge { get; private set; }

        #endregion
    }
}