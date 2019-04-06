namespace Prover.Core.Models.Instruments
{
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.Items;
    using Prover.Core.Extensions;
    using SuperFactorCalculations;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="SuperFactorTest" />
    /// </summary>
    public sealed class SuperFactorTest : BaseVerificationTest
    {
        #region Fields

        /// <summary>
        /// Defines the _factorCalculator
        /// </summary>
        private readonly FactorCalculations _factorCalculator;

        /// <summary>
        /// Defines the _actualFactor
        /// </summary>
        private decimal? _actualFactor;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperFactorTest"/> class.
        /// </summary>
        /// <param name="verificationTest">The verificationTest<see cref="VerificationTest"/></param>
        public SuperFactorTest(VerificationTest verificationTest)
        {
            if (verificationTest == null)
                throw new NullReferenceException(nameof(verificationTest));

            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsSuperFactor == true).ToList();

            VerificationTest = verificationTest;

            _factorCalculator = new FactorCalculations(
                (double)VerificationTest.Instrument.SpecGr().Value,
                (double)VerificationTest.Instrument.CO2().Value,
                (double)VerificationTest.Instrument.N2().Value,
                0,
                0);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ActualFactor
        /// </summary>
        public override decimal? ActualFactor
        {
            get
            {
                if (_actualFactor == null)
                    Calculate();

                return _actualFactor;
            }
        }

        /// <summary>
        /// Gets the EvcFactor
        /// </summary>
        public override decimal? EvcFactor => EvcUnsqrFactor;

        /// <summary>
        /// Gets the EvcUnsqrFactor
        /// </summary>
        [NotMapped]
        public decimal? EvcUnsqrFactor => PressureTest?.Items.GetItem(ItemCodes.Pressure.UnsqrFactor).NumericValue;

        /// <summary>
        /// Gets the GaugePressure
        /// </summary>
        [NotMapped]
        public decimal? GaugePressure => PressureTest?.GasGaugePsi;

        /// <summary>
        /// Gets the GaugeTemp
        /// </summary>
        [NotMapped]
        public decimal GaugeTemp => TemperatureTest.GaugeFahrenheit;

        /// <summary>
        /// Gets the InstrumentType
        /// </summary>
        [NotMapped]
        public override EvcDevice InstrumentType => VerificationTest.Instrument.InstrumentType;

        /// <summary>
        /// Gets the SuperFactorSquared
        /// </summary>
        public decimal? SuperFactorSquared => ActualFactor.HasValue ? (decimal?)Math.Pow((double)ActualFactor, 2) : null;

        /// <summary>
        /// Gets the PassTolerance
        /// </summary>
        protected override decimal PassTolerance => Global.SUPER_FACTOR_TOLERANCE;

        /// <summary>
        /// Gets the PressureTest
        /// </summary>
        private PressureTest PressureTest => VerificationTest.PressureTest;

        /// <summary>
        /// Gets the TemperatureTest
        /// </summary>
        private TemperatureTest TemperatureTest => VerificationTest.TemperatureTest;

        #endregion

        #region Methods

        /// <summary>
        /// The Calculate
        /// </summary>
        public void Calculate()
        {
            _actualFactor = CalculateFpv();
        }

        /// <summary>
        /// The CalculateFpv
        /// </summary>
        /// <returns>The <see cref="decimal"/></returns>
        private decimal? CalculateFpv()
        {
            if (!GaugePressure.HasValue)
                return null;

            _factorCalculator.GaugeTemp = (double)GaugeTemp;
            _factorCalculator.GaugePressure = (double)GaugePressure.Value;

            return decimal.Round((decimal)_factorCalculator.SuperFactor, 4);
        }

        #endregion
    }
}
