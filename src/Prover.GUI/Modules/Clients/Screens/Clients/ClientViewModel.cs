using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Models.Clients;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Controls;
using ReactiveUI;


namespace Prover.GUI.Modules.Clients.Screens.Clients
{
    public class ClientViewModel : ViewModelBase
    {
        private readonly IProverStore<Core.Models.Clients.Client> _clientStore;

        public ClientViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IProverStore<Core.Models.Clients.Client> clientStore, Core.Models.Clients.Client client = null)
            : base(screenManager, eventAggregator)
        {
            _clientStore = clientStore;
            _client = client;

            EditCommand = ReactiveCommand.CreateFromTask(Edit);
            SaveCommand = ReactiveCommand.CreateFromTask(Save);
            GoBackCommand = ReactiveCommand.CreateFromTask(GoBack);

            InstrumentTypes = new List<InstrumentType>(Instruments.GetAll().ToList());                         

            VerifyItemList = new ItemsFileViewModel(_client, ClientItemType.Verify, "These items will be compared to the instruments values at the beginning of the test");
            this.WhenAnyValue(x => x.SelectedInstrumentType)
                .Where(x => x != null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(VerifyItemList.UpdateListItems);

            ResetItemList = new ItemsFileViewModel(_client, ClientItemType.Reset, "These items will be written to the instrument after the volume test is completed.");
            this.WhenAnyValue(x => x.SelectedInstrumentType)
                .Where(x => x != null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(ResetItemList.UpdateListItems);
        }   

        public async Task Edit()
        {
            await ScreenManager.ChangeScreen(this);
            SelectedInstrumentType = InstrumentTypes.First();
            SelectedItemFileType = ClientItemType.Reset;
        }

        private async Task GoBack()
        {
            await Save();
            await ScreenManager.ChangeScreen<ClientManagerViewModel>();
        }

        private async Task Save()
        {
            await _clientStore.UpsertAsync(Client);  
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

        private ReactiveCommand _goToCsvExporter;

        public ReactiveCommand GoToCsvExporter
        {
            get { return _goToCsvExporter; }
            set { this.RaiseAndSetIfChanged(ref _goToCsvExporter, value); }
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

        public List<ClientItemType> ItemFileTypesList
            => Enum.GetValues(typeof(ClientItemType)).Cast<ClientItemType>().ToList();

        private Core.Models.Clients.Client _client;

        public Core.Models.Clients.Client Client
        {
            get { return _client; }
            set { this.RaiseAndSetIfChanged(ref _client, value); }
        }

        //private ReactiveList<ItemValue> _resetItems = new ReactiveList<ItemValue>();

        //public ReactiveList<ItemValue> ResetItems
        //{
        //    get { return _resetItems; }
        //    set { this.RaiseAndSetIfChanged(ref _resetItems, value); }
        //}

        //private ReactiveList<ItemValue> _verifyItems = new ReactiveList<ItemValue>();

        //public ReactiveList<ItemValue> VerifyItems
        //{
        //    get { return _verifyItems; }
        //    set { this.RaiseAndSetIfChanged(ref _verifyItems, value); }
        //}

        //private ReactiveList<ItemMetadata> _items = new ReactiveList<ItemMetadata>();

        //public ReactiveList<ItemMetadata> Items
        //{
        //    get { return _items; }
        //    set { this.RaiseAndSetIfChanged(ref _items, value); }
        //}


        //private readonly ObservableAsPropertyHelper<bool> _showItemDescriptions;
        //public bool ShowItemDescriptions => _showItemDescriptions.Value;

        //private readonly ObservableAsPropertyHelper<bool> _showItemValueTextBox;
        //public bool ShowItemValueTextBox => _showItemValueTextBox.Value;

        //private object _itemValue;

        //public object ItemValue
        //{
        //    get { return _itemValue; }
        //    set { this.RaiseAndSetIfChanged(ref _itemValue, value); }
        //}

        //private ItemMetadata.ItemDescription _selectedItemDescription;

        //public ItemMetadata.ItemDescription SelectedItemDescription
        //{
        //    get { return _selectedItemDescription; }
        //    set { this.RaiseAndSetIfChanged(ref _selectedItemDescription, value); }
        //}

        //private readonly ObservableAsPropertyHelper<List<ItemMetadata.ItemDescription>> _itemDescriptionsList;
        //public List<ItemMetadata.ItemDescription> ItemDescriptionsList => _itemDescriptionsList.Value;

        //private ItemMetadata _selectedItem;

        //public ItemMetadata SelectedItem
        //{
        //    get { return _selectedItem; }
        //    set { this.RaiseAndSetIfChanged(ref _selectedItem, value); }
        //}

        //private IEnumerable<string> _itemStrings;

        //public IEnumerable<string> ItemStrings
        //{
        //    get { return _itemStrings; }
        //    set { this.RaiseAndSetIfChanged(ref _itemStrings, value); }
        //}

        //private ClientItems _currentClientItems;

        //public ClientItems CurrentClientItems
        //{
        //    get { return _currentClientItems; }
        //    set { this.RaiseAndSetIfChanged(ref _currentClientItems, value); }
        //}

        private ItemsFileViewModel _resetItemList;

        public ItemsFileViewModel ResetItemList
        {
            get { return _resetItemList; }
            set { this.RaiseAndSetIfChanged(ref _resetItemList, value); }
        }

        private ItemsFileViewModel _verifyItemList;

        public ItemsFileViewModel VerifyItemList
        {
            get { return _verifyItemList; }
            set { this.RaiseAndSetIfChanged(ref _verifyItemList, value); }
        }


        #endregion

        protected override void OnDeactivate(bool close)
        {
            Save().ContinueWith(_ =>
            {
                base.OnDeactivate(close);
            });
            
        }
    }
}