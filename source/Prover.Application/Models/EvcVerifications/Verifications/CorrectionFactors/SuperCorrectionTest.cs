using System;
using Devices.Core.Items.ItemGroups;
using Prover.Calculations;

namespace Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors
{
    public class SuperCorrectionTest : CorrectionVerificationTest<SuperFactorItems>
    {
        private SuperFactorCalculator _calc;
        protected SuperCorrectionTest(){}

        //public SuperCorrectionTest(SuperFactorItems items, decimal expectedValue, decimal actualValue, decimal gaugePressure, decimal gaugeTemp, decimal percentError)
        //    : base(items, expectedValue, actualValue, percentError)
        //{
        //    GaugePressure = gaugePressure;
        //    GaugeTemp = gaugeTemp;

        //    _calc = new SuperFactorCalculator(items.Co2, items.N2, items.SpecGr, GaugeTemp, GaugePressure);
        //}

        public SuperCorrectionTest(SuperFactorItems items, TemperatureCorrectionTest temperatureCorrectionTest, PressureCorrectionTest pressureCorrectionTest)
        {
            Items = items;
            ActualValue = Items.Factor;
            GaugePressure = pressureCorrectionTest.Gauge;
            GaugeTemp = temperatureCorrectionTest.Gauge;
        
            Update(Tolerances.SUPER_FACTOR_TOLERANCE);
        }

        #region Public Properties

        public decimal GaugePressure { get; private set; }
        public decimal GaugeTemp { get; private set; }


        #endregion


        /// <inheritdoc />
        protected override Func<ICorrectionCalculator> CalculatorFactory => () =>
                new SuperFactorCalculator(Items.Co2, Items.N2, Items.SpecGr, GaugePressure, GaugeTemp);
    }
}