using System.ComponentModel.DataAnnotations.Schema;

namespace Prover.Core.Models.Instruments
{
    public abstract class BaseVerificationTest : ProverTable
    {
        [NotMapped]
        public bool HasPassed => PercentError.HasValue && PercentError < 1 && PercentError > -1;

        public abstract decimal? PercentError { get; }
        public abstract decimal? ActualFactor { get; }
    }
}