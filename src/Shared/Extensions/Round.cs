using System;

namespace Shared.Extensions
{
    public static class Round
    {
        public static decimal Factor(decimal value)
        {
            return Math.Round(value, FactorDecimalPlaces);
        }

        public static decimal Gauge(decimal value)
        {
            return Math.Round(value, GaugeDecimalPlaces);
        }

        private const int FactorDecimalPlaces = 4;
        private const int GaugeDecimalPlaces = 2;
    }
}