using Core.Domain;
using System;

namespace Module.EvcVerification.Models.CorrectionTestAggregate
{
    public interface ICompareFactors
    {
        #region Properties

        double ActualFactor { get; }

        double EvcFactor { get; }

        #endregion
    }

    public interface IHavePercentError
    {
        #region Properties

        bool HasPassed { get; }

        double PassTolerance { get; }

        double? PercentError { get; }

        #endregion
    }

    public abstract class BaseCorrectionTest : BaseEntity, IHavePercentError
    {
        #region Properties

        public abstract double ActualFactor { get; }

        public abstract double EvcFactor { get; }

        public bool HasPassed => PercentError.HasValue && PercentError < PassTolerance && PercentError > -PassTolerance;

        public abstract double PassTolerance { get; }

        public virtual double? PercentError => CalculatePercentError(EvcFactor, ActualFactor);

        #region Methods

        protected double? CalculatePercentError(double? evcValue, double? actualValue)
        {
            if (actualValue == 0)
            {
                return default(double?);
            }

            var result = (evcValue - actualValue) / actualValue;
            return result != null
                ? Math.Round(result.Value * 100, 2)
                : default(double?);
        }

        #endregion

        #endregion
    }
}