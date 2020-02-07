using System;
using Core.GasCalculations.Helpers;
using Devices.Core;
using Devices.Core.Items.ItemGroups;

namespace Core.GasCalculations
{
    public class PressureCalculator
    {
        private readonly IPressureItems _values;

        public PressureCalculator(IPressureItems values)
        {
            _values = values;
        }

        #region Public Methods

        /// <summary>
        ///     Gets the GasPressure
        /// </summary>
        /// <param name="gasGauge"></param>
        /// <param name="atmosphericGauge"></param>
        public decimal GasPressure(decimal gasGauge, decimal atmosphericGauge)
        {
            var result = gasGauge + atmosphericGauge;
            return Math.Round(result, 4);
        }

        public static decimal Factor(decimal baseValue, decimal gasValue)
        {
            if (baseValue == 0)
                throw new DivideByZeroException(nameof(baseValue));

            //var psiGas = gasValue.ConvertToPsi(units);
            //var psiBase = baseValue.ConvertToPsi(units);

            return Round.Factor(gasValue / baseValue);
        }

        public static decimal GetGaugePressure(int range, decimal percentOfGauge)
        {
            if (percentOfGauge > 1)
                percentOfGauge /= 100;

            return Math.Round(percentOfGauge * range, 2);
        }

        #endregion
    }

    public static class PressureUnitExtensions
    {
        #region Public Methods

        public static decimal ConvertToPsi(decimal value, PressureUnitType fromUnit)
        {
            decimal result;
            switch (fromUnit)
            {
                case PressureUnitType.BAR:
                    result = value / (6.894757m * (10 ^ -2));
                    break;

                case PressureUnitType.inWC:
                    result = value / 27.68067m;
                    break;

                case PressureUnitType.KGcm2:
                    result = value / (7.030696m * (10 ^ -2));
                    break;

                case PressureUnitType.kPa:
                    result = value / 6.894757m;
                    break;

                case PressureUnitType.mBAR:
                    result = value / (6.894757m * (10 ^ 1));
                    break;

                case PressureUnitType.mPa:
                    result = value / (6.894757m * (10 ^ -3));
                    break;

                case PressureUnitType.inHG:
                    result = value / 2.03602m;
                    break;

                case PressureUnitType.mmHG:
                    result = value / 51.71492m;
                    break;

                default:
                    result = value / 1;
                    break;
            }

            return Round.Gauge(result);
        }

        public static decimal ConvertToUnits(decimal value, PressureUnitType from, PressureUnitType to)
        {
            var result = ConvertToPsi(value, from);

            switch (to)
            {
                case PressureUnitType.kPa:
                    result *= 6.894757m;
                    break;

                case PressureUnitType.mPa:
                    result *= 6.894757m * (10 ^ -3);
                    break;

                case PressureUnitType.BAR:
                    result *= 6.894757m * (10 ^ -2);
                    break;

                case PressureUnitType.mBAR:
                    result *= 6.894757m * (10 ^ 1);
                    break;

                case PressureUnitType.KGcm2:
                    result *= 7.030696m * (10 ^ -2);
                    break;

                case PressureUnitType.inWC:
                    result *= 27.68067m;
                    break;

                case PressureUnitType.inHG:
                    result *= 2.03602m;
                    break;

                case PressureUnitType.mmHG:
                    result *= 51.71492m;
                    break;
            }

            return Round.Gauge(result);
        }

        #endregion
    }
}