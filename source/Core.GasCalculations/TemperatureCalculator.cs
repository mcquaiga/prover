using Prover.Shared;
using Prover.Shared.Extensions;

namespace Core.GasCalculations
{
    public class TemperatureCalculator : ICorrectionCalculator
    {
        private const decimal MetricTempCorrection = 273.15m;

        private const decimal TempCorrection = 459.67m;
        private readonly decimal _baseTemperature;
        private readonly TemperatureUnitType _unitType;

        public TemperatureCalculator(TemperatureUnitType unitType, decimal baseTemperature, decimal gauge)
        {
            Gauge = gauge;
            _unitType = unitType;
            _baseTemperature = baseTemperature;
        }

        #region Public Properties

        public decimal Gauge { get; }

        #endregion

        #region Public Methods

        public decimal CalculateFactor()
        {
            var correction = GetCorrectionValue(_unitType);

            //var baseF = Items.Base.ConvertTemperatureToFahrenheit(Items.Units);
            //var gaugeF = Gauge.ConvertTemperatureToFahrenheit(Items.Units);

            return Round.Factor(
                (_baseTemperature + correction) / (Gauge + correction)
            );
        }

      

        #endregion

        #region Protected

        protected static decimal GetCorrectionValue(TemperatureUnitType units)
        {
            if (units == TemperatureUnitType.K || units == TemperatureUnitType.C)
                return MetricTempCorrection;

            return TempCorrection;
        }

        #endregion
    }

    public static class TemperatureExtensions
    {
        #region Public Methods

        public static decimal ConvertTemperatureTo(this decimal value, TemperatureUnitType fromUnit,
            TemperatureUnitType toUnit)
        {
            var result = ConvertTemperatureToFahrenheit(value, fromUnit);

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
                    result = (result - 32) / 1.8m + 273.15m;
                    break;
            }

            return Round.Gauge(result);
        }

        /// <summary>
        ///     The ConvertToFahrenheit
        /// </summary>
        /// <param name="value">The value<see cref="decimal" /></param>
        /// <param name="fromUnit">The fromUnit<see cref="string" /></param>
        /// <returns>The <see cref="decimal" /></returns>
        public static decimal ConvertTemperatureToFahrenheit(this decimal value, TemperatureUnitType fromUnit)
        {
            switch (fromUnit)
            {
                case TemperatureUnitType.C:
                    value = value * 1.8m + 32;
                    break;

                case TemperatureUnitType.F:
                    break;

                case TemperatureUnitType.R:
                    value = value - 459.67m;
                    break;

                case TemperatureUnitType.K:
                    value = (value - 273.15m) * 1.8m + 32;
                    break;
            }

            return Round.Gauge(value);
        }

        #endregion
    }
}