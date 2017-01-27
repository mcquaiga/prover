using System;
using ReactiveUI;

namespace Prover.Modules.Clients.Screens.Clients.Designer
{
    public class DesignTimeClientManagerViewModel
    {
        public DesignTimeClientManagerViewModel()
        {
            ClientList = new ReactiveList<Prover.Core.Models.Clients.Client>()
            {
                new Prover.Core.Models.Clients.Client()
                {
                    Id = Guid.NewGuid(),
                    Name = "Adam",
                    Address = "1028 Fraser"
                },
                new Prover.Core.Models.Clients.Client()
                {
                    Id = Guid.NewGuid(),
                    Name = "C.R. Wall",
                    Address = "Thompson Drive, Cambridge"
                }
            };
        }

        public ReactiveList<Core.Models.Clients.Client> ClientList { get; set; }
    }
}
