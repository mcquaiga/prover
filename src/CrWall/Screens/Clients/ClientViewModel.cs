using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
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
            UpdateItemList(Instruments.MiniAt, ItemType.Verify);
        }

        private async Task Save()
        {
            await _clientStore.UpsertAsync(this.Client);
            await ScreenManager.ChangeScreen<ClientManagerViewModel>();
        }

        private void UpdateItemList(InstrumentType instrumentType, ItemType itemFileType)
        {
            var itemList =
                Client.Items.FirstOrDefault(x => x.InstrumentType == instrumentType && x.ItemFileType == itemFileType);

            CurrentItemData = itemList != null ? itemList.Items : new List<ItemValue>();
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
        
        #region Properties   

        public IEnumerable<SelectableViewModel<InstrumentType>> InstrumentTypes
        {
            get
            {
                var instruments = new List<SelectableViewModel<InstrumentType>>();
                Instruments.GetAll().ToList().ForEach(x => instruments.Add(new SelectableViewModel<InstrumentType>(x)));
                return instruments;
            }
        }

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
        #endregion

    }
}