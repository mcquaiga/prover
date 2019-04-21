namespace Module.EvcVerification.Models.CorrectionTestAggregate
{
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="TemperatureTest" />
    /// </summary>
    public sealed class TemperatureTest : BaseCorrectionTest
    {
        #region Constants

        /// <summary>
        /// Defines the MetericTempCorrection
        /// </summary>
        private const double MetericTempCorrection = 273.15m;

        /// <summary>
        /// Defines the TempCorrection
        /// </summary>
        private const double TempCorrection = 459.67m;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureTest"/> class.
        /// </summary>
        private TemperatureTest() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureTest"/> class.
        /// </summary>
        /// <param name="verificationTest">The verificationTest<see cref="CorrectionTest"/></param>
        /// <param name="gauge">The gauge<see cref="double"/></param>
        public TemperatureTest(CorrectionTest verificationTest, double gauge)
        {
            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsTemperatureTest == true).ToList();
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;

            Gauge = (double)ConvertTo(gauge, "F", VerificationTest.Instrument.TemperatureUnits());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ActualFactor
        /// </summary>
        public override double? ActualFactor
        {
            get
            {
                var result = default(double?);
                if (VerificationTest.Instrument.TemperatureUnits() == "K" || VerificationTest.Instrument.TemperatureUnits() == "C")
                {
                    result =
                        (MetericTempCorrection + VerificationTest.Instrument.EvcBaseTemperature().GetValueOrDefault(0)) /
                             ((double)Gauge + MetericTempCorrection);
                }
                else if (VerificationTest.Instrument.TemperatureUnits() == "R" ||
                         VerificationTest.Instrument.TemperatureUnits() == "F")
                {
                    result =
                        (TempCorrection + VerificationTest.Instrument.EvcBaseTemperature().GetValueOrDefault(0)) /
                        ((double)Gauge + TempCorrection);
                }

                return result.HasValue ? double.Round(result.Value, 4) : default(double?);
            }
        }

        /// <summary>
        /// Gets the EvcFactor
        /// </summary>
        public override double? EvcFactor => Items.GetItem(ItemCodes.Temperature.Factor)?.NumericValue;

        /// <summary>
        /// Gets or sets the Gauge
        /// </summary>
        public double Gauge { get; set; }

        /// <summary>
        /// Gets the GaugeFahrenheit
        /// </summary>
        [NotMapped]
        public double GaugeFahrenheit => ConvertToFahrenheit((double)Gauge, VerificationTest.Instrument.TemperatureUnits());

        /// <summary>
        /// Gets the InstrumentType
        /// </summary>
        public override InstrumentType EvcDeviceType => VerificationTest.Instrument.InstrumentType;

        /// <summary>
        /// Gets the PassTolerance
        /// </summary>
        protected override double PassTolerance => Global.TEMP_ERROR_TOLERANCE;

        #endregion

        #region Methods

        /// <summary>
        /// The ConvertTo
        /// </summary>
        /// <param name="value">The value<see cref="double"/></param>
        /// <param name="fromUnit">The fromUnit<see cref="string"/></param>
        /// <param name="toUnit">The toUnit<see cref="string"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double ConvertTo(double value, string fromUnit, string toUnit)
        {
            var result = ConvertToFahrenheit(value, fromUnit);

            //convert from F to new unit
            switch (toUnit)
            {
                case "C":
                    result = (result - 32) / 1.8m;
                    break;
                case "K":
                    result = ((result - 32) / 1.8m) + 273.15m;
                    break;
                case "R":
                    result = result + 459.67m;
                    break;
            }

            return double.Round(result, 2);
        }

        /// <summary>
        /// The ConvertToFahrenheit
        /// </summary>
        /// <param name="value">The value<see cref="double"/></param>
        /// <param name="fromUnit">The fromUnit<see cref="string"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double ConvertToFahrenheit(double value, string fromUnit)
        {
            switch (fromUnit)
            {
                case "C":
                    value = (value * 1.8m) + 32;
                    break;
                case "K":
                    value = ((value - 273.15m) * 1.8m) + 32;
                    break;
                case "R":
                    value = value - 459.67m;
                    break;
            }

            return double.Round(value, 2);
        }

        #endregion
    }
}
