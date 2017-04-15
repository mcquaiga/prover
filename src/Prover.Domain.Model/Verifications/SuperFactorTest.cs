using System;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using Prover.Shared.Domain;
using Prover.Shared.Helpers;
using SuperFactorCalculations;

namespace Prover.Domain.Model.Verifications
{
    public class SuperFactorTest : Entity, ISuperFactorItems
    {
        public SuperFactorTest() : base(Guid.NewGuid())
        {
        }

        public double Co2 { get; set; }
        public double GaugePressure { get; set; }

        public double GaugeTemperature { get; set; }
        public double N2 { get; set; }
        public double SpecGr { get; set; }
        public double UnsquaredFactor { get; set; }

        public double CalculatedFactor()
        {
            var super = new FactorCalculations(
                SpecGr,
                Co2,
                N2,
                GaugeTemperature,
                GaugePressure);

            return Math.Round(super.SuperFactor, 4);
        }

        public double? PercentError()
        {
            return CalculationHelpers.CalculatePercentError(CalculatedFactor(), UnsquaredFactor);
        }
    }
}