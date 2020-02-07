using Core.GasCalculations.Helpers;
using Devices.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.GasCalculations
{
    public class TemperatureCalculator
    {
        public decimal ConvertTo(decimal value, TemperatureUnitType fromUnit, TemperatureUnitType toUnit)
        {
            var result = ConvertToFahrenheit(value, fromUnit);

            switch (toUnit)
            {
                case TemperatureUnitType.C:
                    result = (result - 32) / 1.8m;
                    break;

                case TemperatureUnitType.F:
                    break;

                case TemperatureUnitType.R:
                    result = result + 459.67m;
                    break;

                case TemperatureUnitType.K:
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
        public decimal ConvertToFahrenheit(decimal value, TemperatureUnitType fromUnit)
        {
            switch (fromUnit)
            {
                case TemperatureUnitType.C:
                    value = (value * 1.8m) + 32;
                    break;

                case TemperatureUnitType.F:
                    break;

                case TemperatureUnitType.R:
                    value = value - 459.67m;
                    break;

                case TemperatureUnitType.K:
                    value = ((value - 273.15m) * 1.8m) + 32;
                    break;

                default:
                    break;
            }

            return Round.Gauge(value);
        }

        public decimal CalculateFactor(TemperatureUnitType units, decimal baseValue, decimal gaugeValue)
        {
            var correction = GetCorrectionValue(units);

            return Round.Factor(
                        (baseValue + correction) / (gaugeValue + correction)
                    );
        }

        //public decimal CalculateFactor(TemperatureUnitType units, decimal baseTemperature, decimal gauge)
        //{
        //    var result = 0.0m;
        //    if (units == TemperatureUnitType.K || units == TemperatureUnitType.C)
        //        result = (MetericTempCorrection + baseTemperature) / (gauge + MetericTempCorrection);
        //    else if (units == TemperatureUnitType.R ||
        //             units == TemperatureUnitType.F)
        //        result =
        //            (TempCorrection + baseTemperature) / (gauge + TempCorrection);

        //    return Round.Factor(result);
        //}

        private const decimal MetericTempCorrection = 273.15m;

        private const decimal TempCorrection = 459.67m;

        private decimal GetCorrectionValue(TemperatureUnitType units)
        {
            if (units == TemperatureUnitType.K || units == TemperatureUnitType.C)
            {
                return MetericTempCorrection;
            }
            else
                return TempCorrection;
        }
    }
}