using System;
using SuperFactorCalculations;

namespace Prover.Domain.Verification.TestPoints.SuperFactor
{
    public class SuperFactorTestPoint : TestPointBase<ISuperFactorItems>
    {
        public SuperFactorTestPoint() : base(Guid.Empty)
        {
        }

        public SuperFactorTestPoint(ISuperFactorItems evcItems)
            : base(Guid.Empty, evcItems)
        {
            EvcItems = evcItems;
        }

        public SuperFactorTestPoint(ISuperFactorItems evcItems, double gaugeTemperature, double gaugePressure,
            double unsquaredFactor)
            : this(evcItems)
        {
            GaugeTemperature = gaugeTemperature;
            GaugePressure = gaugePressure;
            UnsquaredFactor = unsquaredFactor;
        }

        public double ActualFactor => Math.Round(CalculateFpv(), 4);
        public double GaugePressure { get; }

        public double GaugeTemperature { get; }

        public double? PercentError
        {
            get
            {
                if (ActualFactor == 0) return default(double?);

                return Math.Round((UnsquaredFactor - ActualFactor) / ActualFactor * 100, 2);
            }
        }

        public double SquaredFactor => Math.Pow(ActualFactor, 2);
        public double UnsquaredFactor { get; }

        public SuperFactorTestPoint Update(double gaugeTemperature, double gaugePressure, double unsqrFactor)
        {
            return new SuperFactorTestPoint(EvcItems, gaugeTemperature, gaugePressure, unsqrFactor);
        }

        private double CalculateFpv()
        {
            var super = new FactorCalculations(
                EvcItems.SpecGr,
                EvcItems.Co2,
                EvcItems.N2,
                GaugeTemperature,
                GaugePressure);

            return super.SuperFactor;
        }
    }
}