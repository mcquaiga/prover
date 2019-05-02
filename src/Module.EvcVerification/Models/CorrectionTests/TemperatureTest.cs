namespace Module.EvcVerification.Models.CorrectionTests
{
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="TemperatureTest" />
    /// </summary>
    public sealed class TemperatureTest : CorrectionBase
    {
        /// <summary>
        /// Defines the MetericTempCorrection
        /// </summary>
        private const decimal MetericTempCorrection = 273.15M;

        /// <summary>
        /// Defines the TempCorrection
        /// </summary>
        private const decimal TempCorrection = 459.67M;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureTest"/> class.
        /// </summary>
        private TemperatureTest() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureTest"/> class.
        /// </summary>
        /// <param name="verificationTest">The verificationTest<see cref="CorrectionTestPoint"/></param>
        /// <param name="gauge">The gauge<see cref="decimal"/></param>
        public TemperatureTest(CorrectionTestPoint verificationTest, decimal gauge)
        {
            Gauge = (decimal)ConvertTo(gauge, "F", VerificationTest.Instrument.TemperatureUnits());
        }

        /// <summary>
        /// Gets the ActualFactor
        /// </summary>
        public override decimal ActualFactor
        {
            get
            {
                var result = default(decimal?);
                if (VerificationTest.Instrument.TemperatureUnits() == "K" || VerificationTest.Instrument.TemperatureUnits() == "C")
                {
                    result =
                        (MetericTempCorrection + VerificationTest.Instrument.EvcBaseTemperature().GetValueOrDefault(0)) /
                             ((decimal)Gauge + MetericTempCorrection);
                }
                else if (VerificationTest.Instrument.TemperatureUnits() == "R" ||
                         VerificationTest.Instrument.TemperatureUnits() == "F")
                {
                    result =
                        (TempCorrection + VerificationTest.Instrument.EvcBaseTemperature().GetValueOrDefault(0)) /
                        ((decimal)Gauge + TempCorrection);
                }

                return result.HasValue ? decimal.Round(result.Value, 4) : default(decimal?);
            }
        }

        /// <summary>
        /// Gets the EvcFactor
        /// </summary>
        public override decimal EvcFactor => Items.GetItem(ItemCodes.Temperature.Factor)?.NumericValue;

        /// <summary>
        /// Gets or sets the Gauge
        /// </summary>
        public decimal Gauge { get; set; }

        public decimal GaugeFahrenheit => ConvertToFahrenheit((decimal)Gauge, VerificationTest.Instrument.TemperatureUnits());

        /// <summary>
        /// Gets the PassTolerance
        /// </summary>
        public override decimal PassTolerance => Global.TEMP_ERROR_TOLERANCE;

        /// <summary>
        /// The ConvertTo
        /// </summary>
        /// <param name="value">The value<see cref="decimal"/></param>
        /// <param name="fromUnit">The fromUnit<see cref="string"/></param>
        /// <param name="toUnit">The toUnit<see cref="string"/></param>
        /// <returns>The <see cref="decimal"/></returns>
        public static decimal ConvertTo(decimal value, string fromUnit, string toUnit)
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

            return decimal.Round(result, 2);
        }

        /// <summary>
        /// The ConvertToFahrenheit
        /// </summary>
        /// <param name="value">The value<see cref="decimal"/></param>
        /// <param name="fromUnit">The fromUnit<see cref="string"/></param>
        /// <returns>The <see cref="decimal"/></returns>
        public static decimal ConvertToFahrenheit(decimal value, string fromUnit)
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

            return decimal.Round(value, 2);
        }
    }
}