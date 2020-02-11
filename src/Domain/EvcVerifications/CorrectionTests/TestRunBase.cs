using Devices.Core.Interfaces;
using Domain.Interfaces;
using Shared.Domain;
using Shared.Extensions;

namespace Domain.EvcVerifications.CorrectionTests
{

    //public class TestRunBase : CorrectionTest
    //{
    //    private 
    //    protected TestRunBase(CorrectionFactorTestType testType) : base(testType, )
    //    {
    //    }

    //    #region Public Properties

    //    public decimal Actual { get; set; }
    //    public decimal Expected { get; set; }

    //    #endregion

    //    public abstract decimal PassTolerance { get; }

    //    #region Public Methods

    //    public override bool HasPassed()
    //    {
    //        return Variance(Expected, Actual).IsBetween(PassTolerance);
    //    }

    //    public virtual decimal Variance(decimal expectedValue, decimal actualValue)
    //    {
    //        return expectedValue == 0
    //            ? 100m
    //            : Round.Factor((actualValue - expectedValue) / expectedValue * 100);
    //    }


    //    #endregion
    //}
}