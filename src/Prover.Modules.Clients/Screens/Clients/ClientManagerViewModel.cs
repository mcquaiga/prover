using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace Prover.Modules.Clients.Screens.Clients
{
    public class ClientManagerViewModel : ViewModelBase
    {
        private readonly IProverStore<Prover.Core.Models.Clients.Client> _clientStore;
        private const string ClientListViewContext = "ClientListView";

        public ClientManagerViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Prover.Core.Models.Clients.Client> clientStore) : base(screenManager, eventAggregator)
        {
            _clientStore = clientStore;

            ViewContext = ClientListViewContext;

            LoadClients();

            AddClientCommand = ReactiveCommand.CreateFromTask(AddClient);
        }

        private void LoadClients()
        {
            _clientStore.Query()
                .OrderBy(c => c.Name)
                .ToList()
                .ForEach(x => ClientList.Add(new ClientViewModel(ScreenManager, EventAggregator, _clientStore, x)));
        }

        private async Task AddClient()
        {
            var newClientVm = new ClientViewModel(ScreenManager, EventAggregator, _clientStore, new Prover.Core.Models.Clients.Client());
            await newClientVm.Edit();
        }

        private ReactiveCommand _addClientCommand;
        public ReactiveCommand AddClientCommand
        {
            get { return _addClientCommand; }
            set { this.RaiseAndSetIfChanged(ref _addClientCommand, value); }
        }

        private ReactiveList<ClientViewModel> _clientList = new ReactiveList<ClientViewModel>();
        public ReactiveList<ClientViewModel> ClientList
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
