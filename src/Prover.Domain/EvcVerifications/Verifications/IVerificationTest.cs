namespace Prover.Domain.EvcVerifications.Verifications
{
    public interface IVerificationTest : IVerification
    {
        #region Public Properties
        decimal ExpectedValue { get; }
        decimal ActualValue { get; }

        #endregion
    }
}