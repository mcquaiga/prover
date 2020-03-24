using Devices.Core.Items.ItemGroups;
using Prover.Shared.Domain;

namespace Prover.Domain.EvcVerifications.Verifications
{
    public interface IVerification<out TValue> : IVerification
    {
        TValue ExpectedValue { get; }
        TValue ActualValue { get; }
        TValue PercentError { get; }
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

        protected VerificationEntity(TValue expectedValue, TValue actualValue,
            TValue percentError) : base()
        {
            ExpectedValue = expectedValue;
            ActualValue = actualValue;
            PercentError = percentError;
        }

        #region Public Properties

        public TValue ExpectedValue { get; protected set; }
        public TValue ActualValue { get; protected set; }
        public TValue PercentError { get; protected set; }

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

        #region Public Properties

        #endregion
    }


    public abstract class VerificationTestEntity<T> : VerificationEntity<decimal>
        where T : ItemGroup
    {
        protected VerificationTestEntity() {}
        #region Public Properties

        protected VerificationTestEntity(T items, decimal expectedValue, decimal actualValue, decimal percentError) 
            : base(expectedValue, actualValue, percentError)
        {
            Items = items;
        }

        public T Items { get; protected set; }

        #endregion
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