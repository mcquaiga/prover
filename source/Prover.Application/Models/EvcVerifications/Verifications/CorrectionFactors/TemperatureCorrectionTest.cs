using Devices.Core.Items.ItemGroups;
using Newtonsoft.Json;

namespace Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors
{
    public sealed class TemperatureCorrectionTest : CorrectionVerificationTest<TemperatureItems>
    {
        #region Public Properties

        private TemperatureCorrectionTest() { }

        [JsonConstructor]
        public TemperatureCorrectionTest(TemperatureItems items, decimal gauge, decimal expectedValue, decimal actualValue, decimal percentError, bool verified)

        {
            Items = items;
            Gauge = gauge;
            ActualValue = items.Factor;

            ExpectedValue = expectedValue;
            ActualValue = actualValue;
            PercentError = percentError;
            Verified = verified;
        }

        public TemperatureCorrectionTest(TemperatureItems items, decimal gaugeTemperature)
        {
            Items = items;
            Gauge = gaugeTemperature;
            ActualValue = items.Factor;

            //Update(Tolerances.TEMP_ERROR_TOLERANCE);
        }


        public decimal Gauge { get; set; }

        #endregion

        ///// <inheritdoc />
        //protected sealed override Func<ICorrectionCalculator> CalculatorFactory
        //    => () => new TemperatureCalculator(Items.Units, Items.Base, Gauge);
    }
}