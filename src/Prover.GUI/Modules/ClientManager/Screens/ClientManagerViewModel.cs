using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Clients;
using Prover.Core.Services;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace Prover.GUI.Modules.ClientManager.Screens
{
    public class ClientManagerViewModel : ViewModelBase, IDisposable
    {
        private readonly ClientService _clientService;
        private const string ClientListViewContext = "ClientListView";

        public ClientManagerViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            ClientService clientService) : base(screenManager, eventAggregator)
        {
            _clientService = clientService;
            ViewContext = ClientListViewContext;

            LoadClientsCommand = ReactiveCommand.CreateFromObservable(LoadClients);
            LoadClientsCommand.Subscribe(client =>
            {
                ClientList.Add(new ClientDetailsViewModel(ScreenManager, EventAggregator, _clientService, client));
            });

            ClientList.ItemChanged
                .Where(x => x.PropertyName == "IsRemoved" && x.Sender.IsRemoved)
                .Select(x => x.Sender)
                .Subscribe(x => ClientList.Remove(x));

            AddClientCommand = ReactiveCommand.CreateFromTask(AddClient);
        }

        public ReactiveCommand<Unit, Client> LoadClientsCommand { get; set; }

        #region Private Functions

        private IObservable<Client> LoadClients()
        {
            return _clientService.GetActiveClients()
                .ToObservable();            
        }

        private async Task AddClient()
        {
            var newClientVm = new ClientDetailsViewModel(ScreenManager, EventAggregator, _clientService,
                new Core.Models.Clients.Client());
            await newClientVm.Edit();
        }

        #endregion

        #region Commands

        private ReactiveCommand _addClientCommand;

        public ReactiveCommand AddClientCommand
        {
            get { return _addClientCommand; }
            set { this.RaiseAndSetIfChanged(ref _addClientCommand, value); }
        }

        #endregion

        #region Properties

        private ReactiveList<ClientDetailsViewModel> _clientList = new ReactiveList<ClientDetailsViewModel>() { ChangeTrackingEnabled = true };

        public ReactiveList<ClientDetailsViewModel> ClientList
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

        #endregion

        public override void Dispose()
        {
            ClientList = null;
        }
    }
}