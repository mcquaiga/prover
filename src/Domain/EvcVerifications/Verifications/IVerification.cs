namespace Domain.EvcVerifications.Verifications
{
    public interface IVerification
    {
        #region Public Properties

        string Description { get; }

        bool Verified { get; }

        #endregion
    }
}