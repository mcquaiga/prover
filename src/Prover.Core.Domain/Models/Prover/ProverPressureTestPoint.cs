using System;
using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Domain.Models.Prover
{
    public class ProverPressureTestPoint : Entity<Guid>
    {
        public ProverPressureTestPoint(Dictionary<string, string> evcItems = null) : base(Guid.NewGuid())
        {
            EvcItems = evcItems ?? new Dictionary<string, string>();
            GaugePressure = 0;
            AtmosphericGauge = 0;
        }

        public Dictionary<string, string> EvcItems { get; set; }

        public double GaugePressure { get; set; }

        public double AtmosphericGauge { get; set; }
    }
}