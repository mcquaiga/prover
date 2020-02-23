using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels.Clients
{
    public class ClientManagerViewModel : ViewModelBase, IRoutableViewModel
    {
        public ClientManagerViewModel(IScreenManager screenManager) : base(screenManager)
        {
            HostScreen = screenManager;
        }


        public string UrlPathSegment { get; } = "/ClientManager";
        public IScreen HostScreen { get; }
    }

    
}
