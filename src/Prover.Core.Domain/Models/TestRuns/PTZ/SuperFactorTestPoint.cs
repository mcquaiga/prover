using System;
using Prover.CommProtocol.Common.Instruments;
using Prover.CommProtocol.Common.Items;
using SuperFactorCalculations;

namespace Prover.Domain.Models.TestRuns
{
    public class SuperFactorTestPoint : ISuperFactorItems
    {
        public PressureTestPoint PressureTest { get; set; }
        public TemperatureTestPoint TemperatureTest { get; set; }

        public decimal GaugeTemp => TemperatureTest.GaugeTemperature;
        public decimal GaugePressure => PressureTest.GaugePressure;

        public decimal EvcUnsqrFactor { get; set; }

        public decimal SpecGr { get; set; }
        public decimal Co2 { get; set; }
        public decimal N2 { get; set; }

        public decimal ActualFactor 
            => decimal.Round((decimal)CalculateFpv(), 4);

        public decimal SuperFactorSquared => (decimal)Math.Pow((double)ActualFactor, 2);

        public decimal? PercentError 
            => ActualFactor != 0 ? decimal.Round((EvcUnsqrFactor - ActualFactor) / ActualFactor * 100, 2) : default(decimal?);

        private double CalculateFpv()
        {
            var super = new FactorCalculations((double)SpecGr, (double)Co2, (double)N2, (double)GaugeTemp, (double)GaugePressure);
            return super.SuperFactor;
        }
    }
}