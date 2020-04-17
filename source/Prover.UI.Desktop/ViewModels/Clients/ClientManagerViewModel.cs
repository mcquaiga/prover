using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.UI.Desktop.ViewModels.Clients
{
    public class ClientManagerViewModel : RoutableViewModelBase, IRoutableViewModel
    {
        public ClientManagerViewModel(IScreenManager screenManager) : base(screenManager) =>
            UrlPathSegment = "/ClientManager";
    }
}