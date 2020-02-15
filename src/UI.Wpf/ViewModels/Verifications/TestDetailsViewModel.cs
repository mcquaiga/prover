using ReactiveUI;

namespace Client.Wpf.ViewModels.Verifications
{
    public class TestDetailsViewModel : ViewModelBase, IRoutableViewModel
    {

        public string UrlPathSegment => "/VerificationTests/Details";
        public IScreen HostScreen => ScreenManager;

        public TestDetailsViewModel(IScreenManager screenManager) : base(screenManager)
        {
       
        }
    }
}