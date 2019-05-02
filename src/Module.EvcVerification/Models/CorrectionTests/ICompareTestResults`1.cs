using Core.Domain;
using System;

namespace Module.EvcVerification.Models.CorrectionTests
{
    public interface ICompareTestResults<T> where T : struct
    {
        #region Properties

        T Expected { get; }

        T Actual { get; }

        decimal PercentVariance { get; }
        #endregion
    }
}