using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels.Clients
{
    public class ClientManagerViewModel : RoutableViewModelBase, IRoutableViewModel
    {
        public ClientManagerViewModel(IScreenManager screenManager) : base(screenManager)
        {
            HostScreen = screenManager;
        }


        public override string UrlPathSegment { get; } = "/ClientManager";
        public override IScreen HostScreen { get; }
    }

    
}
