using System;
using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Domain.Models.Prover
{
    public class ProverVolumeTestPoint : Entity<Guid>
    {
        public ProverVolumeTestPoint(Dictionary<string, string> preTestItemData) : base(Guid.NewGuid())
        {
            PreTestItems = preTestItemData;
            PostTestItems = new Dictionary<string, string>();

            AppliedInput = 0;
            PulserA = 0;
            PulserB = 0;
        }

        public double AppliedInput { get; set; }
        public int PulserA { get; set; }
        public int PulserB { get; set; }

        public Dictionary<string, string> PreTestItems { get; set; }
        public Dictionary<string, string> PostTestItems { get; set; }
    }
}