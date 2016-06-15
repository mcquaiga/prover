using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;

namespace Prover.Core.Models.Instruments
{
    public abstract class BaseVerificationTest : ProverTable
    {
        [NotMapped]
        public bool HasPassed => PercentError.HasValue && (PercentError < 1 && PercentError > -1);

        public abstract decimal? PercentError { get; }
        public abstract decimal? ActualFactor { get; }
    }
}
