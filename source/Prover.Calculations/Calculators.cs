using Prover.Shared.Extensions;
using System;

namespace Prover.Calculations
{
    public static class Calculators
    {
        //public static SuperFactorCalculator Calculate(this SuperFactorItems items, decimal gaugeTempF,
        //    decimal gaugePressurePsi)
        //{
        //    return new SuperFactorCalculator(items.Co2, items.N2, items.SpecGr, gaugeTempF, gaugePressurePsi);
        //}

        public static decimal CalculateFactor(decimal co2, decimal n2, decimal specGr, decimal gaugeTempF,
            decimal gaugePressurePsi) =>
            new SuperFactorCalculator(co2, n2, specGr, gaugeTempF, gaugePressurePsi).CalculateFactor();

        public static decimal Deviation(decimal expected, decimal actual) => expected == 0
            ? 100m
            : Round.Factor((actual - expected) / expected);

        public static int Deviation(int expected, int actual) => expected == 0 ? 100 : (actual - expected);

        //public static PressureCalculator GetCalculator(this PressureItems items, decimal gauge,
        //    decimal? atmosphericGauge)
        //{
        //    return new PressureCalculator(items.UnitType, items.TransducerType, items.Base, gauge,
        //        atmosphericGauge ?? items.AtmosphericPressure);
        //}

        //public static decimal CalculateFactor(this PressureItems items, decimal gauge, decimal? atmosphericGauge)
        //{
        //    return GetPressureFactor(items.UnitType, items.TransducerType, items.Base, gauge,
        //        atmosphericGauge ?? items.AtmosphericPressure);
        //}

        //public static decimal GetPressureFactor(PressureUnitType unitType, PressureTransducerType transducerType,
        //    decimal basePressure, decimal gauge, decimal atmosphericGauge) =>
        //    new PressureCalculator(unitType, transducerType, basePressure, gauge, atmosphericGauge)
        //        .CalculateFactor();

        public static decimal PercentDeviation(decimal expected, decimal actual) => expected == 0
            ? 100m
            : Round.Percentage((actual - expected) / expected * 100);

        public static decimal PercentDeviation(int expected, int actual) => expected == 0
            ? 100m
            : Round.Percentage((actual - expected) / expected * 100);

        public static decimal SquaredFactor(decimal factor) => Round.Factor((decimal)Math.Pow((double)factor, 2d));

        public static decimal TotalCorrectionFactor(decimal? tempFactor, decimal? pressureFactor,
            decimal? superFactorSquared)
        {
            var pFactor = pressureFactor ?? 1.0m;
            var tFactor = tempFactor ?? 1.0m;
            var sFactor = superFactorSquared ?? 1.0m;

            return pFactor * tFactor * sFactor;
        }

        //public static TemperatureCalculator GetCalculator(this TemperatureItems items, decimal gauge)
        //{
        //    return new TemperatureCalculator(items.Units, items.Base, gauge);
        //}
    }
}