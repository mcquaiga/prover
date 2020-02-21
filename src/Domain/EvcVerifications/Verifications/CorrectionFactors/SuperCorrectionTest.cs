using Devices.Core.Items.ItemGroups;

namespace Domain.EvcVerifications.Verifications.CorrectionFactors
{
    public class SuperCorrectionTest : VerificationTestEntity<SuperFactorItems>
    {
        protected SuperCorrectionTest(){}

        public SuperCorrectionTest(SuperFactorItems items, decimal expectedValue, decimal actualValue, decimal gaugePressure, decimal gaugeTemp, decimal percentError)
            : base(items, expectedValue, actualValue, percentError)
        {
            GaugePressure = gaugePressure;
            GaugeTemp = gaugeTemp;
        }

        #region Public Properties

        public decimal GaugePressure { get; private set; }
        public decimal GaugeTemp { get; private set; }


        #endregion

       
    }
}