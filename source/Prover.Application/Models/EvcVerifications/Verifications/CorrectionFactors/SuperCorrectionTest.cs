using Devices.Core.Items.ItemGroups;
using Newtonsoft.Json;
using Prover.Calculations;

namespace Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors
{
    public sealed class SuperCorrectionTest : CorrectionVerificationTest<SuperFactorItems>
    {
        private SuperCorrectionTest() { }

        [JsonConstructor]
        private SuperCorrectionTest(SuperFactorItems items, decimal expectedValue, decimal actualValue, decimal gaugePressure, decimal gaugeTemp, decimal percentError, bool verified)

        {
            Items = items;
            ExpectedValue = expectedValue;
            ActualValue = actualValue;
            GaugePressure = gaugePressure;
            GaugeTemp = gaugeTemp;
            PercentError = percentError;
            Verified = verified;
            SquaredFactor = Calculators.SquaredFactor(ExpectedValue);
            // _calc = new SuperFactorCalculator(items.Co2, items.N2, items.SpecGr, GaugeTemp, GaugePressure);
        }

        public SuperCorrectionTest(SuperFactorItems items, TemperatureCorrectionTest temperatureCorrectionTest, PressureCorrectionTest pressureCorrectionTest)
        {
            Items = items;

            ActualValue = Items.Factor;
            GaugePressure = pressureCorrectionTest.Gauge;
            GaugeTemp = temperatureCorrectionTest.Gauge;

            var calc = new SuperFactorCalculator(Items.Co2, Items.N2, Items.SpecGr, GaugeTemp, GaugePressure);
            ExpectedValue = calc.CalculateFactor();
            SquaredFactor = calc.SquaredFactor();

            Update(Tolerances.SUPER_FACTOR_TOLERANCE);
        }

        #region Public Properties


        public decimal GaugePressure { get; set; }
        public decimal GaugeTemp { get; set; }

        public decimal SquaredFactor { get; set; }

        #endregion

        ///// <inheritdoc />
        //protected override Func<ICorrectionCalculator> CalculatorFactory => () =>
        //        new SuperFactorCalculator(Items.Co2, Items.N2, Items.SpecGr, GaugePressure, GaugeTemp);
    }


}