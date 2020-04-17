using Devices.Core.Items.ItemGroups;

namespace Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors
{
    public class TemperatureCorrectionTest : VerificationTestEntity<TemperatureItems>
    {
        #region Public Properties
        protected  TemperatureCorrectionTest() { }
        public TemperatureCorrectionTest(TemperatureItems items, decimal gaugeTemperature, decimal expectedValue, decimal actualValue, decimal percentError) 
            : base(items, expectedValue, actualValue, percentError)
        {
            Gauge = gaugeTemperature;
        }

        public decimal Gauge { get; private set; }

        #endregion
    }
}