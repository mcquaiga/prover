using Devices.Core.Interfaces;
using Domain.Interfaces;
using Shared.Domain;
using Shared.Extensions;

namespace Domain.EvcVerifications.CorrectionTests
{
    public interface ITestRunBase<out T> : IVerificationTest, ICompareTestResults<decimal>
        where T : IItemGroup
    {
        #region Public Properties

        T ItemsStart { get; }
        T ItemsEnd { get; }

        #endregion
    }

    public abstract class VerificationTestBaseEntity : BaseVerificationTest, IVerificationTest, ICompareTestResults<decimal>
    {
        #region Public Properties

        //public abstract decimal PassTolerance { get; }

        #endregion

        #region Public Methods

        public virtual decimal Variance(decimal expectedValue, decimal actualValue)
        {
            return expectedValue == 0
                ? 100m
                : Round.Factor((actualValue - expectedValue) / expectedValue * 100);
        }


        #endregion

        //public abstract void Calculate();
    }

    public abstract class TestRunBase<T> : VerificationTestBaseEntity, ITestRunBase<T>
        where T : IItemGroup
    {
        protected TestRunBase()
        {
        }

        protected TestRunBase(T startValues, T endValues)
        {
            ItemsEnd = endValues;
            ItemsStart = startValues;
        }

        #region Public Properties

        public decimal Actual { get; set; }
        public decimal Expected { get; set; }

        public T ItemsStart { get; set; }
        public T ItemsEnd { get; set; }

        #endregion

        #region Public Methods

        public override bool HasPassed()
        {
            return Variance(Expected, Actual).IsBetween(1);
        }

        #endregion
    }
}