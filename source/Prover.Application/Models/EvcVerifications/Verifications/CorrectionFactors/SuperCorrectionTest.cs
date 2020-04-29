using Devices.Core.Items.ItemGroups;
using Newtonsoft.Json;
using Prover.Calculations;
using Prover.Shared.Extensions;

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

            ActualValue = pressureCorrectionTest.Items.UnsqrFactor;
            GaugePressure = pressureCorrectionTest.GetTotalGauge();
            GaugeTemp = temperatureCorrectionTest.Gauge;

            _calc = new SuperFactorCalculator(Items.Co2, Items.N2, Items.SpecGr, GaugePressure, GaugeTemp);
            ExpectedValue = _calc.CalculateFactor();
            SquaredFactor = _calc.SquaredFactor();

            PercentError = Calculators.PercentDeviation(ExpectedValue, ActualValue);
            Verified = PercentError.IsBetween(Tolerances.SUPER_FACTOR_TOLERANCE);
            //Update(Tolerances.SUPER_FACTOR_TOLERANCE);
            //SquaredFactor = Calculators.SquaredFactor(ExpectedValue);
        }

        #region Public Properties


        public decimal GaugePressure { get; set; }
        public decimal GaugeTemp { get; set; }

        public decimal SquaredFactor { get; set; }
        private SuperFactorCalculator _calc;

        #endregion

        ///// <inheritdoc />
        //protected override Func<ICorrectionCalculator> CalculatorFactory => () =>
        //        new SuperFactorCalculator(Items.Co2, Items.N2, Items.SpecGr, GaugePressure, GaugeTemp);
    }
}