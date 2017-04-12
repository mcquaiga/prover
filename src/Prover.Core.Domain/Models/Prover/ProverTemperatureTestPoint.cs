using System;
using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Domain.Models.Prover
{
    public class ProverTemperatureTestPoint : Entity<Guid>
    {
        public ProverTemperatureTestPoint(Dictionary<string, string> evcItems) : base(Guid.NewGuid())
        {
            EvcItems = evcItems ?? new Dictionary<string, string>();
            GaugeTemperature = 0;
        }

        public Dictionary<string, string> EvcItems { get; set; }

        public double GaugeTemperature { get; set; }

    }
}