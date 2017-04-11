using System;
using Prover.Domain.Instrument.Items;
using Prover.Domain.Verification.TestPoints.Pressure;
using Prover.Domain.Verification.TestPoints.Temperature;
using SuperFactorCalculations;

namespace Prover.Domain.Verification.TestPoints.SuperFactor
{
    public class SuperFactorTestPoint : TestPointBase<ISuperFactorItems>
    {
        public SuperFactorTestPoint() : base(Guid.Empty) { }

        public SuperFactorTestPoint(ISuperFactorItems evcItems)
            : base(Guid.Empty, evcItems)
        {
            EvcItems = evcItems;
        }

        public SuperFactorTestPoint(ISuperFactorItems evcItems, double gaugeTemperature, double gaugePressure, double unsquaredFactor)
            : this(evcItems)
        {
            GaugeTemperature = gaugeTemperature;
            GaugePressure = gaugePressure;
            UnsquaredFactor = unsquaredFactor;
        }

        public double GaugeTemperature { get; private set; }
        public double GaugePressure { get; private set; }
        public double UnsquaredFactor { get; private set; }

        public SuperFactorTestPoint Update(double gaugeTemperature, double gaugePressure, double unsqrFactor)
        {
            return new SuperFactorTestPoint(EvcItems, gaugeTemperature, gaugePressure, unsqrFactor);
        }

        public double ActualFactor => Math.Round(CalculateFpv(), 4);

        public double? PercentError
        {
            get
            {
                if (ActualFactor == 0) return default(double?);

                return Math.Round((UnsquaredFactor - ActualFactor) / ActualFactor * 100, 2);
            }
        }

        public double SquaredFactor => (double) Math.Pow((double) ActualFactor, 2);

        private double CalculateFpv()
        {
            var super = new FactorCalculations(
                (double) EvcItems.SpecGr,
                (double) EvcItems.Co2,
                (double) EvcItems.N2,
                GaugeTemperature,
                GaugePressure);

            return (double)super.SuperFactor;
        }
    }
}