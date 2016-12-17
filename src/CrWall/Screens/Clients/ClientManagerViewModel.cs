using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Clients;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace CrWall.Screens.Clients
{
    public class ClientManagerViewModel : ViewModelBase
    {
        private readonly IProverStore<Client> _clientStore;
        private const string ClientListViewContext = "ClientListView";

        public ClientManagerViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Client> clientStore) : base(screenManager, eventAggregator)
        {
            _clientStore = clientStore;

            ViewContext = ClientListViewContext;

            ClientList.AddRange(clientStore.Query()
                .OrderBy(c => c.Name));
        }

        private ReactiveCommand _addClientCommand;
        public ReactiveCommand

        private ReactiveList<Client> _clientList = new ReactiveList<Client>();
        public ReactiveList<Client> ClientList
        {
            get { return _clientList; }
            set { this.RaiseAndSetIfChanged(ref _clientList, value); }
        }

        private string _viewContext;
        public string ViewContext
        {
            get { return _viewContext; }
            set { this.RaiseAndSetIfChanged(ref _viewContext, value); }
        }
    }
}
