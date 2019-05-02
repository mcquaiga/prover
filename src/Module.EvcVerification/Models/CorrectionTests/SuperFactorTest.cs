namespace Module.EvcVerification.Models.CorrectionTests
{
    using Devices.Core.Interfaces.Items;
    using SuperFactorCalculations;
    using System;

    /// <summary>
    /// Defines the <see cref="SuperFactorTest"/>
    /// </summary>
    public sealed class SuperFactorTest : CorrectionBase
    {
        #region Fields

        /// <summary>
        /// Defines the _factorCalculator
        /// </summary>
        private readonly FactorCalculations _factorCalculator;

        /// <summary>
        /// Defines the _actualFactor
        /// </summary>
        private decimal _actualFactor;

        private IPressureItems PressureItems;

        private ISuperFactorItems SuperFactorItems;

        private ITemperatureItems TemperatureItems;

        #endregion

        #region Constructors

        public SuperFactorTest(ISuperFactorItems superFactorItems, PressureTest pressureTest, TemperatureTest temperatureTest)
        {
            SuperFactorItems = superFactorItems;
            TemperatureTest = temperatureTest;
            PressureTest = pressureTest;

            _factorCalculator = new FactorCalculations(
                (decimal)SuperFactorItems.SpecGr,
                (decimal)SuperFactorItems.Co2,
                (decimal)SuperFactorItems.N2,
                TemperatureTest.GaugeFahrenheit,
                PressureTest.GasPressurePsi);

            CalculateFactor();
        }

        #endregion

        #region Properties

        public override decimal ActualFactor => _actualFactor;

        public override decimal EvcFactor => EvcUnsqrFactor;

        public decimal EvcUnsqrFactor => PressureItems.UnsqrFactor;

        public override decimal PassTolerance => Global.SUPER_FACTOR_TOLERANCE;

        public PressureTest PressureTest { get; }

        public decimal SuperFactorSquared => Math.Pow(ActualFactor, 2);

        public TemperatureTest TemperatureTest { get; }

        #endregion

        #region Methods

        /// <summary>
        /// The CalculateFpv
        /// </summary>
        /// <returns>The <see cref="decimal"/></returns>
        public void CalculateFactor()
        {
            _factorCalculator.GaugeTemp = TemperatureTest.GaugeFahrenheit;
            _factorCalculator.GaugePressure = PressureTest.GasPressurePsi;

            _actualFactor = Math.Round(_factorCalculator.SuperFactor, 4);
        }

        #endregion
    }
}