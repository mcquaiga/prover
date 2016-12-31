using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Clients;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace CrWall.Screens.Clients
{
    public class ClientViewModel : ViewModelBase
    {
        private readonly IProverStore<Prover.Core.Models.Clients.Client> _clientStore;

        public ClientViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, 
            IProverStore<Prover.Core.Models.Clients.Client> clientStore, Prover.Core.Models.Clients.Client client = null) : base(screenManager, eventAggregator)
        {
            _clientStore = clientStore;
            _client = client;

            EditCommand = ReactiveCommand.CreateFromTask(Edit);

            //var canSave = this.WhenAnyValue(x => x.Client,
            //    _ => !string.IsNullOrEmpty(_.Name) && !string.IsNullOrEmpty(_.Address));
            SaveCommand = ReactiveCommand.CreateFromTask(Save);


        }

        public async Task Edit()
        {
            await ScreenManager.ChangeScreen(this);
        }

        private async Task Save()
        {
            await _clientStore.UpsertAsync(this.Client);
            await ScreenManager.ChangeScreen<ClientManagerViewModel>();
        }

        private async Task UpdateItemList()
        {
            
        }

        #region Commands
        private ReactiveCommand _saveCommand;
        public ReactiveCommand SaveCommand
        {
            get { return _saveCommand; }
            set { this.RaiseAndSetIfChanged(ref _saveCommand, value); }
        }

        private ReactiveCommand _editCommand;
        public ReactiveCommand EditCommand
        {
            get { return _editCommand; }
            set { this.RaiseAndSetIfChanged(ref _editCommand, value); }
        }
        #endregion

        private Prover.Core.Models.Clients.Client _client;
        public Prover.Core.Models.Clients.Client Client
        {
            get { return _client; }
            set { this.RaiseAndSetIfChanged(ref _client, value); }
        }

        private IEnumerable<ItemValue> _currentItemValues = new List<ItemValue>();
        public IEnumerable<ItemValue> CurrentItemData
        {
            get { return _currentItemValues; }
            set { this.RaiseAndSetIfChanged(ref _currentItemValues, value); }
        }

    }
}