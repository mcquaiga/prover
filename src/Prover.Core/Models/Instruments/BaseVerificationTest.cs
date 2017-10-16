using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prover.Core.Models.Instruments
{
    public abstract class BaseVerificationTest : ProverTable
    {
        [NotMapped]
        public bool HasPassed => PercentError.HasValue && PercentError < PassTolerance && PercentError > -PassTolerance;

        protected virtual decimal PassTolerance => 1;

        public abstract decimal? PercentError { get; }
        public abstract decimal? ActualFactor { get; }

        public virtual Guid VerificationTestId { get; set; }

        [Required]
        public virtual VerificationTest VerificationTest { get; set; }
    }
}