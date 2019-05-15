using Devices.Core.Interfaces.Items;
using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Models.EvcVerifications.CorrectionTests
{
    public abstract class CorrectionBase<T> : BaseEntity, ICompareTestResults<decimal>, IAssertPassFail
        where T : IItemsGroup
    {
        public abstract decimal Actual { get; }
        public virtual T EvcValues { get; set; }
        public abstract decimal Expected { get; }
        public bool Passed => Variance < PassTolerance && Variance > -PassTolerance;

        public abstract decimal PassTolerance { get; }

        public decimal Variance
        {
            get
            {
                if (Expected == 0)
                {
                    return 100M;
                }

                return (Actual - Expected) / Expected * 100;
            }
        }
    }

    public abstract class TestRunBase<T> : BaseEntity, ICompareTestResults<decimal>, IAssertPassFail
        where T : IItemsGroup
    {
        public abstract decimal Actual { get; }
        public virtual T EndEvcValues { get; set; }
        public abstract decimal Expected { get; }
        public bool Passed => Variance < PassTolerance && Variance > -PassTolerance;
        public abstract decimal PassTolerance { get; }

        public virtual T StartEvcValues { get; set; }

        public decimal Variance
        {
            get
            {
                if (Expected == 0)
                {
                    return 100M;
                }

                return (Actual - Expected) / Expected * 100;
            }
        }
    }
}