using System;
using System.Collections.Generic;
using Prover.Domain.Model.Domain;
using Prover.Domain.Model.Instrument;

namespace Prover.Domain.Model.Verifications
{
    public class VerificationRun : AggregateRoot
    {
        public VerificationRun() : base(Guid.NewGuid())
        {
            TestDateTime = DateTime.UtcNow;
        }

        public DateTime? ArchivedDateTime { get; set; }

        public DateTime? ExportedDateTime { get; set; }
        public virtual EvcInstrument Instrument { get; set; }

        public Guid InstrumentId { get; set; }

        public DateTime TestDateTime { get; set; }

        public virtual ICollection<VerificationRunTestPoint> TestPoints { get; set; }
    }
}