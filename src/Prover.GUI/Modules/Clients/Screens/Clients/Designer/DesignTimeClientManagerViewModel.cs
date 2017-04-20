using ReactiveUI;

namespace Prover.GUI.Modules.Clients.Screens.Clients.Designer
{
    public class DesignTimeClientManagerViewModel
    {
        public DesignTimeClientManagerViewModel()
        {
            //ClientList.Add(new ClientViewModel());
            //{
            //    new Prover.Core.Models.Clients.Client()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Adam",
            //        Address = "1028 Fraser"
            //    },
            //    new Prover.Core.Models.Clients.Client()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "C.R. Wall",
            //        Address = "Thompson Drive, Cambridge"
            //    }
            //};
        }

        public ReactiveList<ClientViewModel> ClientList { get; set; } = new ReactiveList<ClientViewModel>();
    }
}
