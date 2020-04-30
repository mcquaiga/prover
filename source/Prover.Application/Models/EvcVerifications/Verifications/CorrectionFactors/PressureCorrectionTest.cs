using Devices.Core.Items.ItemGroups;
using Newtonsoft.Json;
using Prover.Calculations;
using Prover.Shared;

namespace Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors
{
    public sealed class PressureCorrectionTest : CorrectionVerificationTest<PressureItems>
    {
        private const decimal Tolerance = Tolerances.PRESSURE_ERROR_TOLERANCE;

        private PressureCorrectionTest()
        {
        }

        [JsonConstructor]
        public PressureCorrectionTest(PressureItems items, decimal gauge, decimal atmosphericGauge, decimal actualValue, decimal expectedValue, decimal percentError, bool verified)
        {
            Gauge = gauge;
            AtmosphericGauge = atmosphericGauge;
            ActualValue = actualValue;
            ExpectedValue = expectedValue;
            PercentError = percentError;
            Verified = verified;
            Items = items;
        }

        #region Public Properties

        public PressureCorrectionTest(PressureItems items, decimal gauge, decimal atmosphericGauge)
        {
            Items = items;
            Gauge = gauge;
            AtmosphericGauge = Items.TransducerType == PressureTransducerType.Absolute ? atmosphericGauge : 0m;
            ActualValue = items.Factor;

            ExpectedValue = Calculator.CalculateFactor();
            Update(Tolerance);
        }

        private PressureCalculator Calculator => new PressureCalculator(Items, Gauge, AtmosphericGauge);

        public decimal Gauge { get; set; }
        public decimal AtmosphericGauge { get; set; }

        #endregion
        //public decimal GetTotalGauge()
        //{
        //    return PressureCalculator.GetGasPressure(Items.TransducerType, Gauge, AtmosphericGauge);
        //}

        ///// <inheritdoc />
        //protected Func<ICorrectionCalculator> CalculatorFactory =>
        //    () => new PressureCalculator(Items.UnitType, Items.TransducerType, Items.Base, Gauge, AtmosphericGauge);
    }
}