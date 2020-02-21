using Domain.EvcVerifications;

namespace Application.ViewModels.Volume
{
    public interface IVolumeViewModelFactory
    {
        void CreateRelatedTests(EvcVerificationTest evcVerification);
    }
}