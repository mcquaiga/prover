using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Models.Clients;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReactiveUI.Legacy;
using ReactiveCommand = ReactiveUI.ReactiveCommand;
using ReactiveCommandMixins = ReactiveUI.ReactiveCommandMixins;

namespace Prover.Modules.Clients.Screens.Clients
{
    public class ClientViewModel : ViewModelBase
    {
        private readonly IProverStore<Core.Models.Clients.Client> _clientStore;

        public ClientViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, 
            IProverStore<Prover.Core.Models.Clients.Client> clientStore, Prover.Core.Models.Clients.Client client = null) : base(screenManager, eventAggregator)
        {
            _clientStore = clientStore;
            _client = client;
           
            EditCommand = ReactiveCommand.CreateFromTask(Edit);
            SaveCommand = ReactiveCommand.CreateFromTask(Save);
            GoBackCommand = ReactiveCommand.CreateFromTask(GoBack);

            InstrumentTypes = new List<InstrumentType>(Instruments.GetAll().ToList());
            UpdateItemListCommand = ReactiveCommand.CreateFromTask<Tuple<InstrumentType, ClientItemType>>(UpdateItemList);
            ReactiveCommandMixins.InvokeCommand(this.WhenAnyValue(x => x.SelectedInstrumentType, x => x.SelectedItemFileType)
                    .Where(x => x.Item1 != null & x.Item2 != null), UpdateItemListCommand);      

            var canAddItem = this.WhenAnyValue(x => x.SelectedItem, x => x.ItemValue,
                (selectedItem, itemValue) => selectedItem != null && itemValue != null);
            AddItemCommand = ReactiveCommand.CreateFromTask(AddItem, canAddItem);

            DeleteRowCommand = ReactiveCommand.Create<ItemValue>(x =>
            {
                CurrentItemData.Remove(x);
                CurrentClientItems.Items.Remove(x);
            });

            CurrentItemData.ResetChangeThreshold = 90;
        }
        
        private async Task AddItem()
        {
            var itemValue = new ItemValue(SelectedItem, ItemValue.ToString());
            CurrentClientItems.Items.Add(itemValue);
            CurrentItemData.Add(itemValue);
            SelectedItem = null;
            ItemValue = null;         
        }

        public async Task Edit()
        {
            await ScreenManager.ChangeScreen(this);
            SelectedInstrumentType = InstrumentTypes.First();
            SelectedItemFileType = ClientItemType.Reset;
        }

        private async Task GoBack()
        {
            await ScreenManager.ChangeScreen<ClientManagerViewModel>();
        }

        private async Task Save()
        {
            await _clientStore.UpsertAsync(this.Client);
            await GoBack();
        }

        private async Task UpdateItemList(Tuple<InstrumentType, ClientItemType> values)
        {
            using (CurrentItemData.SuppressChangeNotifications())
            {
                CurrentItemData.Clear();

                Items.Clear();
                Items.AddRange(ItemHelpers.LoadItems(SelectedInstrumentType));

                if (GetItemList(SelectedInstrumentType, SelectedItemFileType) == null)
                {
                    var items = new ClientItems(Client)
                    {
                        InstrumentType = SelectedInstrumentType,
                        ItemFileType = SelectedItemFileType,
                        Items = new List<ItemValue>()
                    };
                    _client.Items.Add(items);
                }
                CurrentClientItems = GetItemList(SelectedInstrumentType, SelectedItemFileType);
                CurrentItemData.AddRange(CurrentClientItems.Items.OrderBy(x => x.Metadata.Number));
            }
        }

        private ClientItems GetItemList(InstrumentType instrumentType, ClientItemType clientItemType)
        {
             return _client.Items.FirstOrDefault(x => x.InstrumentType == instrumentType && x.ItemFileType == clientItemType);
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
        
        private ReactiveCommand _updateItemListCommand;
        public ReactiveCommand UpdateItemListCommand
        {
            get { return _updateItemListCommand; }
            set { this.RaiseAndSetIfChanged(ref _updateItemListCommand, value); }
        }

        private ReactiveCommand _goBackCommand;
        public ReactiveCommand GoBackCommand
        {
            get { return _goBackCommand; }
            set { this.RaiseAndSetIfChanged(ref _goBackCommand, value); }
        }

        private ReactiveCommand _deleteRowCommand;
        public ReactiveCommand DeleteRowCommand
        {
            get { return _deleteRowCommand; }
            set { this.RaiseAndSetIfChanged(ref _deleteRowCommand, value); }
        }

        private ReactiveCommand _addItemCommand;
        public ReactiveCommand AddItemCommand
        {
            get { return _addItemCommand; }
            set { this.RaiseAndSetIfChanged(ref _addItemCommand, value); }
        }
        #endregion

        #region Properties   
        private IEnumerable<InstrumentType> _instrumentTypes;
        public IEnumerable<InstrumentType> InstrumentTypes
        {
            get { return _instrumentTypes; }
            set { this.RaiseAndSetIfChanged(ref _instrumentTypes, value); }
        }

        private InstrumentType _selecedInstrumentType;
        public InstrumentType SelectedInstrumentType
        {
            get { return _selecedInstrumentType; }
            set { this.RaiseAndSetIfChanged(ref _selecedInstrumentType, value); }
        }

        private ClientItemType _selectedClientItemFileType;
        public ClientItemType SelectedItemFileType
        {
            get { return _selectedClientItemFileType; }
            set { this.RaiseAndSetIfChanged(ref _selectedClientItemFileType, value); }
        }

        public List<ClientItemType> ItemFileTypesList => Enum.GetValues(typeof(ClientItemType)).Cast<ClientItemType>().ToList();

        private Prover.Core.Models.Clients.Client _client;
        public Prover.Core.Models.Clients.Client Client
        {
            get { return _client; }
            set { this.RaiseAndSetIfChanged(ref _client, value); }
        }

        private ReactiveList<ItemValue> _currentItemValues = new ReactiveList<ItemValue>();
        public ReactiveList<ItemValue> CurrentItemData
        {
            get { return _currentItemValues; }
            set { this.RaiseAndSetIfChanged(ref _currentItemValues, value); }
        }

        private ReactiveList<ItemMetadata> _items = new ReactiveList<ItemMetadata>();
        public ReactiveList<ItemMetadata> Items
        {
            get { return _items; }
            set { this.RaiseAndSetIfChanged(ref _items, value); }
        }

        //private ItemValue _itemValue;
        //public ItemValue ItemValue
        //{
        //    get { return _itemValue; }
        //    set { this.RaiseAndSetIfChanged(ref _itemValue, value); }
        //}
        private object _itemValue;
        public object ItemValue
        {
            get { return _itemValue; }
            set { this.RaiseAndSetIfChanged(ref _itemValue, value); }
        }

        private ItemMetadata _selectedItem;
        public ItemMetadata SelectedItem
        {
            get { return _selectedItem; }
            set { this.RaiseAndSetIfChanged(ref _selectedItem, value); }
        }

        private IEnumerable<string> _itemStrings;
        public IEnumerable<string> ItemStrings
        {
            get { return _itemStrings; }
            set { this.RaiseAndSetIfChanged(ref _itemStrings, value); }
        }

        private ClientItems _currentClientItems;
        public ClientItems CurrentClientItems
        {
            get { return _currentClientItems; }
            set { this.RaiseAndSetIfChanged(ref _currentClientItems, value); }
        }

        #endregion

    }
}