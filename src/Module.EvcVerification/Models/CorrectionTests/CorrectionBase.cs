using Core.Domain;

namespace Module.EvcVerification.Models.CorrectionTests
{
    public abstract class CorrectionBase : BaseEntity, ICompareTestResults<decimal>, IAssertPassFail
    {
        protected abstract decimal PassTolerance { get; }
        public decimal Actual { get; }
        public decimal Expected { get; }
        public bool Passed => PercentVariance < PassTolerance && PercentVariance > -PassTolerance;

        public decimal PercentVariance
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