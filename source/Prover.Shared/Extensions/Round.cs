using System;

namespace Prover.Shared.Extensions
{
    public static class Round
    {
        private const int FactorDecimalPlaces = 4;
        private const int GaugeDecimalPlaces = 2;
        private const int PercentDecimalPlaces = 2;
        private const int VolumeDecimalPlaces = 4;

        #region Public Methods

        public static decimal Factor(decimal value)
        {
            return Math.Round(value, FactorDecimalPlaces);
        }

        public static decimal Gauge(decimal value)
        {
            return Math.Round(value, GaugeDecimalPlaces);
        }

        public static decimal Percentage(decimal value)
        {
            return Math.Round(value, PercentDecimalPlaces);
        }

        public static decimal Volume(decimal value)
        {
            return Math.Round(value, VolumeDecimalPlaces);
        }

        #endregion
    }
}