using Devices.Core.Items.ItemGroups;
using Shared.Domain;

namespace Domain.EvcVerifications.Verifications
{
    public enum VerificationTestType
    {
        Super,
        Pressure,
        Temperature,
        CorrectedVolume,
        UncorrectedVolume,
        Energy
    }

    public class VerificationEntity : BaseEntity, IVerification
    {
        protected VerificationEntity() { }

        public VerificationEntity(string description)
        {
            Description = description;
            Verified = false;
        }

        public VerificationEntity(bool verified)
        {
            Verified = verified;
        }

        #region Public Properties

        public string Description { get; private set; }

        public bool Verified { get; protected set; }

        #endregion
    }


    public abstract class VerificationTestEntity : VerificationEntity, IVerificationTest
    {
        protected VerificationTestEntity()
        {
        }

        protected VerificationTestEntity(decimal expectedValue, decimal actualValue,
            decimal percentError, bool verified) : base(verified)
        {
            ExpectedValue = expectedValue;
            ActualValue = actualValue;
            PercentError = percentError;
            Verified = verified;
        }

        #region Public Properties



        public decimal ExpectedValue { get; protected set; }

        public decimal ActualValue { get; protected set; }
        public decimal PercentError { get; protected set; }

        #endregion
    }

    public abstract class VerificationTestEntity<T> : VerificationTestEntity
        where T : ItemGroup
    {
        protected VerificationTestEntity() {}
        #region Public Properties

        protected VerificationTestEntity(T items, decimal expectedValue, decimal actualValue, decimal percentError, bool verified) 
            : base(expectedValue, actualValue, percentError, verified)
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
        #region Public Properties
        protected VerificationTestEntity(TStart startValues, TEnd endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified) 
            : base(expectedValue, actualValue, percentError, verified)
        {
            StartValues = startValues;
            EndValues = endValues;
        }

        public TStart StartValues { get; protected set; }
        public TEnd EndValues { get; protected set; }

        #endregion
    }
}