using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels.Clients
{
    public class ClientManagerViewModel : RoutableViewModelBase, IRoutableViewModel
    {
        public ClientManagerViewModel(IScreenManager screenManager) : base(screenManager) =>
            UrlPathSegment = "/ClientManager";
    }
}