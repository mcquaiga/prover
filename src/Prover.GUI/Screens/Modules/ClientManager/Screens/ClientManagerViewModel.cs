using System;
using System.Reactive;
using System.Reactive.Linq;
using Caliburn.Micro;
using Prover.Core.Models.Clients;
using Prover.Core.Services;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.ClientManager.Screens
{
    public class ClientManagerViewModel : ViewModelBase, IDisposable
    {
        private readonly IClientService _clientService;
        private const string ClientListViewContext = "ClientListView";

        public ClientManagerViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IClientService clientService) : base(screenManager, eventAggregator)
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

            AddClientCommand = ReactiveCommand.Create(AddClient);
        }

        public ReactiveCommand<Unit, Client> LoadClientsCommand { get; set; }

        #region Private Functions

        private IObservable<Client> LoadClients()
        {
            return _clientService.GetActiveClients()
                .ToObservable();
        }

        private void AddClient()
        {
            var newClientVm = new ClientDetailsViewModel(ScreenManager, EventAggregator, _clientService);
            newClientVm.Edit();
        }

        #endregion

        #region Commands

        private ReactiveCommand _addClientCommand;

        public ReactiveCommand AddClientCommand
        {
            get => _addClientCommand;
            set => this.RaiseAndSetIfChanged(ref _addClientCommand, value);
        }

        #endregion

        #region Properties

        private ReactiveList<ClientDetailsViewModel> _clientList =
            new ReactiveList<ClientDetailsViewModel> {ChangeTrackingEnabled = true};

        public ReactiveList<ClientDetailsViewModel> ClientList
        {
            get => _clientList;
            set => this.RaiseAndSetIfChanged(ref _clientList, value);
        }

        private string _viewContext;

        public string ViewContext
        {
            get => _viewContext;
            set => this.RaiseAndSetIfChanged(ref _viewContext, value);
        }

        #endregion

        public override void Dispose()
        {
            ClientList = null;
        }
    }
}