using System;
using Prover.Shared.Domain;
using Prover.Shared.Enums;

namespace Prover.Domain.Model.Verifications
{
    public class VerificationRunTestPoint : Entity
    {
        public VerificationRunTestPoint() : base(Guid.NewGuid())
        {
        }

        public VerificationRunTestPoint(TestLevel level) : base(Guid.NewGuid())
        {
            Level = level;
        }

        public TestLevel Level { get; set; }
        public virtual PressureTest Pressure { get; set; }

        public Guid? PressureId { get; set; }
        public virtual TemperatureTest Temperature { get; set; }

        public Guid? TemperatureId { get; set; }
        public virtual VerificationRun VerificationRun { get; set; }

        public Guid VerificationRunId { get; set; }
        public virtual VolumeTest Volume { get; set; }

        public Guid? VolumeId { get; set; }

        //public virtual SuperFactorTestPoint SuperFactor { get; set; }
    }
}