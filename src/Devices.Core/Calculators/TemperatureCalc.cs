using Devices.Core.Calculators.Helpers;

namespace Devices.Core.Calculators
{
    public static class Temperature
    {
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