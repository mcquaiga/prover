namespace Module.EvcVerification.Models.CorrectionTestAggregate
{
    using SuperFactorCalculations;
    using System;

    /// <summary>
    /// Defines the <see cref="SuperFactorTest"/>
    /// </summary>
    public sealed class SuperFactorTest : BaseCorrectionTest
    {
        #region Fields

        /// <summary>
        /// Defines the _factorCalculator
        /// </summary>
        private readonly FactorCalculations _factorCalculator;

        /// <summary>
        /// Defines the _actualFactor
        /// </summary>
        private double? _actualFactor;

        private IPressureItems PressureItems;

        private ISuperFactorItems SuperFactorItems;

        private ITemperatureItems TemperatureItems;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperFactorTest"/> class.
        /// </summary>
        /// <param name="verificationTest">The verificationTest <see cref="CorrectionTest"/></param>
        public SuperFactorTest(CorrectionTest verificationTest)
        {
            VerificationTest = verificationTest;
        }

        public SuperFactorTest(ISuperFactorItems superFactorItems, PressureTest pressureTest, TemperatureTest temperatureTest)
        {
            SuperFactorItems = superFactorItems;
            TemperatureTest = temperatureTest;
            PressureTest = pressureTest;

            _factorCalculator = new FactorCalculations(
                (double)SuperFactorItems.SpecGr,
                (double)SuperFactorItems.Co2,
                (double)SuperFactorItems.N2,
                TemperatureTest.GaugeFahrenheit,
                PressureTest.GasPressurePsi);
        }

        #endregion

        #region Properties

        public override double ActualFactor { get; private set; }

        public override double EvcFactor => EvcUnsqrFactor;

        public double EvcUnsqrFactor => PressureItems.UnsqrFactor;

        public PressureTest PressureTest { get; }

        public double SuperFactorSquared => Math.Pow(ActualFactor, 2);

        public TemperatureTest TemperatureTest { get; }

        protected override double PassTolerance => Global.SUPER_FACTOR_TOLERANCE;

        #endregion

        #region Methods

        /// <summary>
        /// The CalculateFpv
        /// </summary>
        /// <returns>The <see cref="double"/></returns>
        public void CalculateFactor()
        {
            _factorCalculator.GaugeTemp = TemperatureTest.GaugeFahrenheit;
            _factorCalculator.GaugePressure = PressureTest.GasPressurePsi;

            ActualFactor = Math.Round(_factorCalculator.SuperFactor, 4);
        }

        #endregion
    }
}