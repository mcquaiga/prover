using System;
using Domain.Calculators.Helpers;
using Domain.Interfaces;

namespace Domain.Models.EvcVerifications.CorrectionTests
{
    public class PulseOutputChannel : IAssertPassFail, ICompareTestResults<int>
    {
        public int Actual { get; set; }

        public int Expected { get; set; }

        public bool Passed => Math.Abs(Expected - Actual).IsBetween(Global.PULSE_VARIANCE_THRESHOLD);

        public int Variance => (Expected - Actual) / Actual * 100;
    }
}