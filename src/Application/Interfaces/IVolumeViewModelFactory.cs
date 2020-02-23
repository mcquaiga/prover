using System;
using Domain.EvcVerifications;

namespace Application.Interfaces
{
    public interface IVolumeViewModelFactory
    {
        void CreateRelatedTests(EvcVerificationTest evcVerification);
    }

    public interface ICalculateTrueCorrectedFactor
    {
        IObservable<decimal> TrueCorrectedObservable { get; }
        decimal TotalCorrectionFactor { get; }
    }
}