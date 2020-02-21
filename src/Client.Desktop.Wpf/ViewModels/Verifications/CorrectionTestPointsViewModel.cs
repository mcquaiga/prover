using Application.ViewModels;
using ReactiveUI;

namespace Client.Wpf.ViewModels.Verifications
{
    public class CorrectionTestPointsViewModel : ReactiveObject
    {
        public VerificationTestPointViewModel ViewModel { get; }

        public CorrectionTestPointsViewModel(VerificationTestPointViewModel verificationTestPointViewModel)
        {
            ViewModel = verificationTestPointViewModel;
        }
    }
}