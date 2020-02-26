using Devices.Core.Items.ItemGroups;
using Prover.Shared.Domain;

namespace Prover.Domain.EvcVerifications.Verifications
{
    public class VerificationEntity : BaseEntity, IVerification
    {
        public VerificationEntity() { }

        #region Public Properties


        public virtual bool Verified { get; protected set; } = false;

        #endregion
    }

    public abstract class VerificationEntity<TValue> : VerificationEntity
      
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
    }

    public abstract class VerificationTestEntity : VerificationEntity<decimal>, IVerificationTest
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


    public abstract class VerificationTestEntity<T> : VerificationEntity<decimal>, IVerificationTest
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