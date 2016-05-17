﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Instruments
{
    public interface IVerificationTest
    {
        decimal? PercentError { get; }
        bool HasPassed { get; }
        decimal? ActualFactor { get; }
    }

    public abstract class BaseVerificationTest : ProverTable, IVerificationTest
    {
        [NotMapped]
        public bool HasPassed => PercentError.HasValue && (PercentError < 1 && PercentError > -1);

        public abstract decimal? PercentError { get; }
        public abstract decimal? ActualFactor { get; }
    }
}