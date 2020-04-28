using Devices.Core.Items.ItemGroups;
using Prover.Calculations;
using Prover.Shared.Domain;
using Prover.Shared.Extensions;
using System;

namespace Prover.Application.Models.EvcVerifications.Verifications
{
    public interface IVerification<out TValue> : IVerification
    {
        TValue ExpectedValue { get; }
        TValue ActualValue { get; }
        decimal PercentError { get; }
    }

    public class VerificationEntity : BaseEntity, IVerification
    {
        public virtual bool Verified { get; set; }
    }

    public abstract class VerificationEntity<TValue> : VerificationEntity, IVerification<TValue>
    {
        protected VerificationEntity()
        {
        }

        protected VerificationEntity(TValue expectedValue, TValue actualValue, decimal percentError) : base()
        {
            ExpectedValue = expectedValue;
            ActualValue = actualValue;
            PercentError = percentError;
        }

        #region Public Properties

        public TValue ExpectedValue { get; protected set; }
        public TValue ActualValue { get; protected set; }
        public decimal PercentError { get; protected set; }


        #endregion

        //public bool Verified { get; set; }
    }

    public abstract class VerificationTestEntity : VerificationEntity<decimal>
    {
        protected VerificationTestEntity()
        {
        }

        protected VerificationTestEntity(decimal expectedValue, decimal actualValue,
            decimal percentError) : base(expectedValue, actualValue, percentError)
        {
        }


        protected virtual void Update(decimal passTolerance)
        {
            PercentError = Calculators.PercentDeviation(ExpectedValue, ActualValue);
            Verified = PercentError.IsBetween(passTolerance);
        }

        #region Public Properties

        #endregion
    }


    public abstract class VerificationTestEntity<T> : VerificationTestEntity
        where T : ItemGroup
    {
        protected VerificationTestEntity() { }
        #region Public Properties

        protected VerificationTestEntity(T items, decimal expectedValue, decimal actualValue, decimal percentError)
            : base(expectedValue, actualValue, percentError)
        {
            Items = items;
        }

        public T Items { get; protected set; }


        #endregion
    }

    public abstract class CorrectionVerificationTest<T> : VerificationTestEntity<T>
        where T : ItemGroup, ICorrectionFactor
    {
        protected CorrectionVerificationTest() { }


        protected abstract Func<ICorrectionCalculator> CalculatorFactory { get; }

        protected override void Update(decimal passTolerance)
        {
            ExpectedValue = CalculatorFactory.Invoke()
                                             .CalculateFactor();

            base.Update(passTolerance);
        }
    }


    public abstract class VerificationTestEntity<TStart, TEnd> : VerificationTestEntity
        where TStart : ItemGroup
        where TEnd : ItemGroup
    {
        protected VerificationTestEntity()
        {

        }

        #region Public Properties
        protected VerificationTestEntity(TStart startValues, TEnd endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified = false)
            : base(expectedValue, actualValue, percentError)
        {
            StartValues = startValues;
            EndValues = endValues;

        }

        public TStart StartValues { get; protected set; }
        public TEnd EndValues { get; protected set; }

        #endregion
    }
}