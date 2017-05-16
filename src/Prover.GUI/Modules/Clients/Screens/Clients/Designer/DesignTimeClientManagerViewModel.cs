using ReactiveUI;

namespace Prover.GUI.Modules.Clients.Screens.Clients.Designer
{
    public class DesignTimeClientManagerViewModel
    {
        public ReactiveList<ClientViewModel> ClientList { get; set; } = new ReactiveList<ClientViewModel>();
    }
}