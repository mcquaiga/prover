using Devices.Core.Interfaces.Items;
using Domain.Interfaces;
using Shared.Domain;

namespace Domain.Models.EvcVerifications.CorrectionTests
{
    public abstract class CorrectionBase<T> : BaseEntity, ICompareTestResults<decimal>, IAssertPassFail
        where T : IItemGroup
    {
        protected CorrectionBase(T values, decimal actual, decimal expected)
        {
            Values = values;
            Actual = actual;
            Expected = expected;
        }

        #region Public Properties

        public decimal Actual { get; }
        public T Values { get; }
        public decimal Expected { get; }
        public bool Passed => Variance < PassTolerance && Variance > -PassTolerance;

        public abstract decimal PassTolerance { get; }

        public decimal Variance
        {
            get
            {
                if (Expected == 0) return 100M;

                return (Actual - Expected) / Expected * 100;
            }
        }

        #endregion
    }

    public abstract class TestRunBase<T> : BaseEntity, ICompareTestResults<decimal>, IAssertPassFail
        where T : IItemGroup
    {
        protected TestRunBase(T startValues, T endValues, decimal actual, decimal expected)
        {
            Actual = actual;
            Expected = expected;
            EndValues = endValues;
            StartValues = startValues;
        }

        #region Public Properties

        public decimal Actual { get; }
        public T EndValues { get; }
        public decimal Expected { get; }
        public virtual bool Passed => Variance < PassTolerance && Variance > -PassTolerance;
        public virtual decimal PassTolerance { get; }

        public T StartValues { get; }

        public virtual decimal Variance
        {
            get
            {
                if (Expected == 0) return 100M;

                return (Actual - Expected) / Expected * 100;
            }
        }

        #endregion
    }
}