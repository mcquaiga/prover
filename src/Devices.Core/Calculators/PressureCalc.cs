using Devices.Core.Calculators.Helpers;
using System;

namespace Devices.Core.Calculators
{
    public static class PressureCalc
    {
        public static decimal ConvertToPsi(this decimal value, PressureUnits fromUnit)
        {
            decimal result;
            switch (fromUnit.ToString().ToLower())
            {
                case "bar":
                    result = value / (6.894757m * (10 ^ -2));
                    break;

                case "inwc":
                    result = value / 27.68067m;
                    break;

                case "kgcm2":
                    result = value / (7.030696m * (10 ^ -2));
                    break;

                case "kpa":
                    result = value / 6.894757m;
                    break;

                case "mbar":
                    result = value / (6.894757m * (10 ^ 1));
                    break;

                case "mpa":
                    result = value / (6.894757m * (10 ^ -3));
                    break;

                case "inhg":
                    result = value / 2.03602m;
                    break;

                case "mmhg":
                    result = value / 51.71492m;
                    break;

                default:
                    result = value / 1;
                    break;
            }

            return Round.Gauge(result);
        }

        public static decimal ConvertToUnits(this decimal value, PressureUnits from, PressureUnits to)
        {
            var result = ConvertToPsi(value, from);

            switch (to)
            {
                case PressureUnits.kPa:
                    result *= 6.894757m;
                    break;

                case PressureUnits.mPa:
                    result *= (6.894757m * (10 ^ -3));
                    break;

                case PressureUnits.BAR:
                    result *= (6.894757m * (10 ^ -2));
                    break;

                case PressureUnits.mBAR:
                    result *= (6.894757m * (10 ^ 1));
                    break;

                case PressureUnits.KGcm2:
                    result *= (7.030696m * (10 ^ -2));
                    break;

                case PressureUnits.inWC:
                    result *= 27.68067m;
                    break;

                case PressureUnits.inHG:
                    result *= 2.03602m;
                    break;

                case PressureUnits.mmHG:
                    result *= 51.71492m;
                    break;
            }

            return Round.Gauge(result);
        }

        public static decimal Factor(decimal baseValue, decimal gasValue)
        {
            if (baseValue == 0)
                throw new DivideByZeroException(nameof(baseValue));

            //var psiGas = gasValue.ConvertToPsi(units);
            //var psiBase = baseValue.ConvertToPsi(units);

            return Round.Factor(gasValue / baseValue);
        }
    }
}