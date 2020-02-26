using System;
using Prover.Shared;
using Prover.Shared.Extensions;

namespace Core.GasCalculations
{
    public class PressureCalculator : ICorrectionCalculator
    {
        private readonly decimal _basePressure;

        private readonly PressureTransducerType _transducerType;
        private readonly PressureUnitType _unitType;

        public PressureCalculator(PressureUnitType unitType, PressureTransducerType transducerType,
            decimal basePressure, decimal gaugePressure, decimal atmPressure)
        {
            _unitType = unitType;
            _transducerType = transducerType;
            _basePressure = basePressure;

            Gauge = gaugePressure;
            Atmospheric = atmPressure;
        }

        #region Public Properties

        public decimal Gauge { get; }
        public decimal Atmospheric { get; }

        public decimal GasPressure => GetGasPressure(_transducerType, Gauge, Atmospheric);

        #endregion

        #region Public Methods

        public decimal CalculateFactor()
        {
            if (_basePressure == 0)
                return 0;

            var psiGas = GasPressure.ConvertToPsi(_unitType);
            var psiBase = _basePressure.ConvertToPsi(_unitType);

            return Round.Factor(psiGas / psiBase);
        }

        

        /// <summary>
        ///     Gets the GasPressure
        /// </summary>
        /// <param name="transducerType"></param>
        /// <param name="gauge"></param>
        /// <param name="atmosphericGauge"></param>
        /// <param name="items"></param>
        public static decimal GetGasPressure(PressureTransducerType transducerType, decimal gauge,
            decimal atmosphericGauge)
        {
            switch (transducerType)
            {
                case PressureTransducerType.Gauge:
                    return Round.Gauge(gauge + atmosphericGauge);

                case PressureTransducerType.Absolute:
                    return Round.Gauge(gauge);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static decimal GetGaugePressure(int range, decimal percentOfRange)
        {
            if (percentOfRange > 1)
                percentOfRange /= 100;

            return Round.Gauge(percentOfRange * range);
        }

        #endregion
    }

    public static class PressureUnitExtensions
    {
        #region Public Methods

        public static decimal ConvertToPsi(this decimal value, PressureUnitType fromUnit)
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