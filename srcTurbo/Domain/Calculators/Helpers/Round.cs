using System;

namespace Domain.Calculators.Helpers
{
    internal static class Round
    {
        internal static decimal Factor(decimal value)
        {
            return Math.Round(value, FactorDecimalPlaces);
        }

        internal static decimal Gauge(decimal value)
        {
            return Math.Round(value, GaugeDecimalPlaces);
        }

        private const int FactorDecimalPlaces = 4;
        private const int GaugeDecimalPlaces = 2;
    }
}