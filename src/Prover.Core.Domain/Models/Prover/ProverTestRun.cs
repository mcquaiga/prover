using System;
using System.Collections.Generic;
using Prover.Domain.Instrument;
using Prover.Domain.Models.Instrument;
using Prover.Shared.Domain;

namespace Prover.Domain.Models.Prover
{
    public class ProverTestRun : AggregateRoot<Guid>
    {
        public ProverTestRun() : base(Guid.NewGuid())
        {
            TestDateTime = DateTime.UtcNow;
        }

        public virtual IInstrument Instrument { get; set; }

        public DateTime? ArchivedDateTime { get; set; }

        public DateTime? ExportedDateTime { get; set; }

        public DateTime TestDateTime { get; set; }

        public virtual ICollection<ProverTestPoint> TestPoints { get; set; }
    }
}