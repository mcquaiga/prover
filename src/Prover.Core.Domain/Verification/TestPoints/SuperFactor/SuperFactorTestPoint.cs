using System;
using Prover.Domain.Instrument.Items;
using Prover.Domain.Verification.TestPoints.Pressure;
using Prover.Domain.Verification.TestPoints.Temperature;
using SuperFactorCalculations;

namespace Prover.Domain.Verification.TestPoints.SuperFactor
{
    public class SuperFactorTestPoint
    {
        private readonly PressureTestPoint _pressureTest;
        private readonly TemperatureTestPoint _temperatureTest;

        public SuperFactorTestPoint(ISuperFactorItems superFactorItems, PressureTestPoint pressureTest, TemperatureTestPoint temperatureTest)
        {
            EvcItems = superFactorItems;
            _pressureTest = pressureTest;
            _temperatureTest = temperatureTest;
        }

        public decimal ActualFactor => decimal.Round((decimal) CalculateFpv(), 4);

        public ISuperFactorItems EvcItems { get; set; }

        public decimal? PercentError
            =>
                ActualFactor != 0
                    ? decimal.Round((_pressureTest.EvcItems.UnsqrFactor - ActualFactor) / ActualFactor * 100, 2)
                    : default(decimal?);

        public decimal SquaredFactor => (decimal) Math.Pow((double) ActualFactor, 2);

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