using System;
using Domain.Interfaces;
using Shared.Extensions;

namespace Domain.EvcVerifications.CorrectionTests
{
    public class PulseOutputChannel : IAssertPassFail, ICompareTestResults<int>
    {
        #region Public Properties

        public int ActualValue { get; set; }

        public int ExpectedValue { get; set; }

        public bool HasPassed() => Math.Abs(ExpectedValue - ActualValue).IsBetween(Global.PULSE_VARIANCE_THRESHOLD);
        
        #endregion
        public int Variance(int expectedValue, int actualValue)
        {
            throw new NotImplementedException();
        }
    }
}