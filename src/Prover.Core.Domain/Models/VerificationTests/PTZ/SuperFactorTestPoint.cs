using System;
using Prover.Domain.Models.Instruments.Items;
using SuperFactorCalculations;

namespace Prover.Domain.Models.VerificationTests.PTZ
{
    public class SuperFactorTestPoint
    {
        private readonly PressureTestPoint _pressureTest;
        private readonly TemperatureTestPoint _temperatureTest;

        public SuperFactorTestPoint(ISuperFactorItems superFactorItems, PressureTestPoint pressureTest,
            TemperatureTestPoint temperatureTest)
        {
            EvcItems = superFactorItems;
            _pressureTest = pressureTest;
            _temperatureTest = temperatureTest;
        }

        public ISuperFactorItems EvcItems { get; set; }

        public decimal ActualFactor => decimal.Round((decimal) CalculateFpv(), 4);

        public decimal SquaredFactor => (decimal) Math.Pow((double) ActualFactor, 2);

        public decimal? PercentError
            =>
                ActualFactor != 0
                    ? decimal.Round((_pressureTest.EvcItems.UnsqrFactor - ActualFactor) / ActualFactor * 100, 2)
                    : default(decimal?);

        private double CalculateFpv()
        {
            var super = new FactorCalculations(
                (double) EvcItems.SpecGr,
                (double) EvcItems.Co2,
                (double) EvcItems.N2,
                (double) _temperatureTest.GaugeTemperature,
                (double) _pressureTest.GaugePressure);
            return super.SuperFactor;
        }
    }
}