using System;
using Devices.Core.Interfaces.Items;
using Core.GasCalculations.ZFactor;
using Domain.Calculators.Helpers;

namespace Domain.Models.EvcVerifications.CorrectionTests
{
    /// <summary>
    /// Defines the <see cref="SuperFactorTest"/>
    /// </summary>
    public sealed class SuperFactorTest : CorrectionBase<ISuperFactorItems>
    {
        //public override decimal EvcFactor => EvcUnsqrFactor;
        public override decimal Expected => _expectedFactor;

        public override decimal PassTolerance => Global.SUPER_FACTOR_TOLERANCE;
        public decimal SuperFactorSquared => (decimal)Math.Pow((double)Actual, 2);
        public override decimal Actual => _pressureTest.Actual;

        //public override decimal ActualFactor => _actualFactor;
        public SuperFactorTest(ISuperFactorItems superFactorItems, PressureTest pressureTest, TemperatureTest temperatureTest)
        {
            _pressureTest = pressureTest;
            _temperatureTest = temperatureTest;
            _factorCalculator = new ZFactorCalc(
                (decimal)superFactorItems.SpecGr,
                (decimal)superFactorItems.Co2,
                (decimal)superFactorItems.N2,
                _temperatureTest.GaugeFahrenheit,
                _pressureTest.GasPressurePsi);
        }

        /// <summary>
        /// The CalculateFpv
        /// </summary>
        /// <returns>The <see cref="decimal"/></returns>
        public void CalculateFactor()
        {
            _factorCalculator.GaugeTemp = _temperatureTest.GaugeFahrenheit;
            _factorCalculator.GaugePressure = _pressureTest.GasPressurePsi;

            _expectedFactor = Round.Factor(_factorCalculator.SuperFactor);
        }

        /// <summary>
        /// Defines the _factorCalculator
        /// </summary>
        private readonly ZFactorCalc _factorCalculator;

        private readonly PressureTest _pressureTest;

        private readonly TemperatureTest _temperatureTest;

        /// <summary>
        /// Defines the _actualFactor
        /// </summary>
        private decimal _expectedFactor;
    }
}