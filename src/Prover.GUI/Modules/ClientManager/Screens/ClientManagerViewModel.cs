using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Clients;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace Prover.GUI.Modules.ClientManager.Screens
{
    public class ClientManagerViewModel : ViewModelBase, IDisposable
    {
        private readonly IClientStore _clientStore;
        private const string ClientListViewContext = "ClientListView";

        public ClientManagerViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IClientStore clientStore) : base(screenManager, eventAggregator)
        {
            _clientStore = clientStore;
            ViewContext = ClientListViewContext;

            LoadClientsCommand = ReactiveCommand.CreateFromObservable(LoadClients);
            LoadClientsCommand.Subscribe(client =>
            {
                ClientList.Add(new ClientDetailsViewModel(ScreenManager, EventAggregator, _clientStore, client));
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
            return _clientStore.Query()
                .Where(x => x.ArchivedDateTime == null)
                .OrderBy(c => c.Name)
                .ToObservable();
            //.ForEach(x => );
        }

        private async Task AddClient()
        {
            var newClientVm = new ClientDetailsViewModel(ScreenManager, EventAggregator, _clientStore,
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

        public void Dispose()
        {
            ClientList = null;
        }
    }
}