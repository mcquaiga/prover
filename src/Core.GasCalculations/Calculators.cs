using System;
using Devices.Core;
using Devices.Core.Items.ItemGroups;
using Shared.Extensions;

namespace Core.GasCalculations
{
    public static class Calculators
    {
        #region Public Methods

        public static decimal Deviation(decimal expected, decimal actual)
        {
            return expected == 0
                ? 100m
                : Round.Factor((actual - expected) / expected);
        }

        #region Temperature

        public static TemperatureCalculator GetCalculator(this TemperatureItems items, decimal gauge)
        {
            return new TemperatureCalculator(items.Units, items.Base, gauge);
        }

        #endregion

        public static decimal PercentDeviation(decimal expected, decimal actual)
        {
            return expected == 0
                ? 100m
                : Round.Percentage((actual - expected) / expected * 100);
        }

        public static decimal PercentDeviation(int expected, int actual)
        {
            return expected == 0
                ? 100m
                : Round.Percentage((actual - expected) / expected * 100);
        }

        public static decimal TotalCorrectionFactor(decimal? tempFactor, decimal? pressureFactor,
            decimal? superFactorSquared)
        {
            var pFactor = pressureFactor ?? 1.0m;
            var tFactor = tempFactor ?? 1.0m;
            var sFactor = superFactorSquared ?? 1.0m;

            return pFactor * tFactor * sFactor;
        }

        #endregion

        #region SuperFactor

        public static SuperFactorCalculator Calculate(this SuperFactorItems items, decimal gaugeTempF,
            decimal gaugePressurePsi)
        {
            return new SuperFactorCalculator(items.Co2, items.N2, items.SpecGr, gaugeTempF, gaugePressurePsi);
        }

        public static decimal CalculateFactor(decimal co2, decimal n2, decimal specGr, decimal gaugeTempF,
            decimal gaugePressurePsi)
        {
            return new SuperFactorCalculator(co2, n2, specGr, gaugeTempF, gaugePressurePsi).CalculateFactor();
        }

        public static decimal SquaredFactor(decimal factor)
        {
            return Round.Factor((decimal) Math.Pow((double) factor, 2d));
        }

        #endregion

        #region Pressure

        public static PressureCalculator GetCalculator(this PressureItems items, decimal gauge,
            decimal? atmosphericGauge)
        {
            return new PressureCalculator(items.UnitType, items.TransducerType, items.Base, gauge,
                atmosphericGauge ?? items.AtmosphericPressure);
        }

        public static decimal CalculateFactor(this PressureItems items, decimal gauge, decimal? atmosphericGauge)
        {
            return GetPressureFactor(items.UnitType, items.TransducerType, items.Base, gauge,
                atmosphericGauge ?? items.AtmosphericPressure);
        }

        public static decimal GetPressureFactor(PressureUnitType unitType, PressureTransducerType transducerType,
            decimal basePressure, decimal gauge, decimal atmosphericGauge)
        {
            return new PressureCalculator(unitType, transducerType, basePressure, gauge, atmosphericGauge)
                .CalculateFactor();
        }

        #endregion
    }
}