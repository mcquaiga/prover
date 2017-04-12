using System;
using Prover.Domain.Verification.TestPoints.SuperFactor;
using Prover.Domain.Verification.TestPoints.Volume;
using Prover.Shared.Domain;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.Prover
{
    public class ProverTestPoint : Entity<Guid>
    {
        public ProverTestPoint(TestLevel level) : base(Guid.NewGuid())
        {
            Level = level;
        }

        public TestLevel Level { get; set; }

        public Guid TestRunId { get; set; }
        public virtual ProverTestRun TestRun { get; set; }
        
        public Guid? PressureId { get; set; }
        public virtual ProverPressureTestPoint Pressure { get; set; }
        
        public Guid? TemperatureId { get; set; }
        public virtual ProverTemperatureTestPoint Temperature { get; set; }

        public Guid? VolumeId { get; set; }
        public virtual ProverVolumeTestPoint Volume { get; set; }

        //public virtual SuperFactorTestPoint SuperFactor { get; set; }
    }
}