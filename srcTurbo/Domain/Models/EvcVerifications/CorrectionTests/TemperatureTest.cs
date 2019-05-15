using Devices.Core;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Domain.Calculators.Helpers;

namespace Domain.Models.EvcVerifications.CorrectionTests
{
    /// <summary>
    /// Defines the <see cref="TemperatureTest" />
    /// </summary>
    public sealed class TemperatureTest : CorrectionBase<ITemperatureItems>
    {
        /// <summary>
        /// Gets the ActualFactor
        /// </summary>
        public override decimal Actual => EvcValues.Factor;

        public override decimal Expected
        {
            get
            {
                var result = 0.0m;
                if (EvcValues.Units == TemperatureUnits.K || EvcValues.Units == TemperatureUnits.C)
                {
                    result =
                        (MetericTempCorrection + EvcValues.Base) /
                        ((decimal)Gauge + MetericTempCorrection);
                }
                else if (EvcValues.Units == TemperatureUnits.R ||
                         EvcValues.Units == TemperatureUnits.F)
                {
                    result =
                        (TempCorrection + EvcValues.Base) / (Gauge + TempCorrection);
                }

                return Round.Factor(result);
            }
        }

        /// <summary>
        /// Gets or sets the Gauge
        /// </summary>
        public decimal Gauge { get; set; }

        public decimal GaugeFahrenheit => ConvertToFahrenheit((decimal)Gauge, EvcValues.Units);

        /// <summary>
        /// Gets the PassTolerance
        /// </summary>
        public override decimal PassTolerance => Global.TEMP_ERROR_TOLERANCE;

        public TemperatureTest(ITemperatureItems items, decimal gauge)
        {
            _items = items;
            Gauge = ConvertTo(gauge, TemperatureUnits.F, EvcValues.Units);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureTest"/> class.
        /// </summary>
        /// <summary>
        /// The ConvertTo
        /// </summary>
        /// <param name="value">The value<see cref="decimal"/></param>
        /// <param name="fromUnit">The fromUnit<see cref="string"/></param>
        /// <param name="toUnit">The toUnit<see cref="string"/></param>
        /// <returns>The <see cref="decimal"/></returns>
        public static decimal ConvertTo(decimal value, TemperatureUnits fromUnit, TemperatureUnits toUnit)
        {
            var result = ConvertToFahrenheit(value, fromUnit);

            switch (toUnit)
            {
                case TemperatureUnits.C:
                    result = (result - 32) / 1.8m;
                    break;

                case TemperatureUnits.F:
                    break;

                case TemperatureUnits.R:
                    result = result + 459.67m;
                    break;

                case TemperatureUnits.K:
                    result = ((result - 32) / 1.8m) + 273.15m;
                    break;

                default:
                    break;
            }

            return Round.Gauge(result);
        }

        /// <summary>
        /// The ConvertToFahrenheit
        /// </summary>
        /// <param name="value">The value<see cref="decimal"/></param>
        /// <param name="fromUnit">The fromUnit<see cref="string"/></param>
        /// <returns>The <see cref="decimal"/></returns>
        public static decimal ConvertToFahrenheit(decimal value, TemperatureUnits fromUnit)
        {
            switch (fromUnit)
            {
                case TemperatureUnits.C:
                    value = (value * 1.8m) + 32;
                    break;

                case TemperatureUnits.F:
                    break;

                case TemperatureUnits.R:
                    value = value - 459.67m;
                    break;

                case TemperatureUnits.K:
                    value = ((value - 273.15m) * 1.8m) + 32;
                    break;

                default:
                    break;
            }

            return Round.Gauge(value);
        }

        public static decimal Factor(TemperatureUnits units, decimal baseValue, decimal gaugeValue)
        {
            var correction = GetCorrectionValue(units);

            return Round.Factor(
                        (baseValue + correction) / (gaugeValue + correction)
                    );
        }

        private const decimal MetericTempCorrection = 273.15m;
        private const decimal TempCorrection = 459.67m;
        private readonly ITemperatureItems _items;

        private TemperatureTest()
        {
        }

        private static decimal GetCorrectionValue(TemperatureUnits units)
        {
            if (units == TemperatureUnits.K || units == TemperatureUnits.C)
            {
                return MetericTempCorrection;
            }
            else
                return TempCorrection;
        }
    }
}