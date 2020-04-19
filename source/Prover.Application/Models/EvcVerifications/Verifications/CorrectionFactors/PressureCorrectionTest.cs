using System;
using Devices.Core.Items.ItemGroups;
using Prover.Calculations;
using Prover.Shared;

namespace Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors
{
    public class PressureCorrectionTest : CorrectionVerificationTest<PressureItems>
    {
        private readonly PressureCalculator _calculator;
        private const decimal Tolerance = Tolerances.PRESSURE_ERROR_TOLERANCE;

        protected PressureCorrectionTest()
        {
        }

        //public PressureCorrectionTest(PressureUnitType unitType, PressureTransducerType transducerType, decimal basePressure, decimal gaugePressure, decimal gaugeAtmospheric, decimal actualFactor)
        //{
        //    Items = new PressureItems()
        //    {
        //        Base = basePressure,
        //        UnitType = unitType,
        //        TransducerType = transducerType
        //    };

        //    ActualValue = actualFactor;

        //    _calculator = new PressureCalculator(unitType, transducerType, basePressure, gaugePressure, gaugeAtmospheric);
            
        //    Update(gaugePressure, gaugeAtmospheric);
        //}

        #region Public Properties

        public PressureCorrectionTest(PressureItems items, decimal gauge, decimal? atmGauge)
        {
            Items = items;
            Gauge = gauge;
            AtmosphericGauge = atmGauge ?? Items.AtmosphericPressure;
            ActualValue = items.Factor;

            Update(Tolerance);
        }

        public decimal Gauge { get; set; }
        public decimal AtmosphericGauge { get; set; }

        #endregion

        //public void Update(decimal? gauge = null, decimal? atmGauge = null)
        //{
        //    Gauge = gauge ?? Gauge;
        //    AtmosphericGauge = atmGauge ?? AtmosphericGauge;

        //    _calculator.Gauge = Gauge;
        //    _calculator.Atmospheric = AtmosphericGauge;
        //    ExpectedValue = _calculator.CalculateFactor();
        //    PercentError = Calculators.PercentDeviation(ExpectedValue, ActualValue);
        //}

        /// <inheritdoc />
        protected override Func<ICorrectionCalculator> CalculatorFactory =>
            () => new PressureCalculator(Items.UnitType, Items.TransducerType, Items.Base, Gauge, AtmosphericGauge);
    }
}