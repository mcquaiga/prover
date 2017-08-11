using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Prover.CommProtocol.Common.Items;
using ReactiveUI;
using System.Reactive.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Models.Clients;

namespace Prover.GUI.Controls
{
    public class ItemsFileViewModel : ReactiveScreen
    {
        private readonly Client _client;
        public ClientItemType ItemsFileType { get; }

        public ItemsFileViewModel(Client client, ClientItemType itemsFileType, string helpInfo)
        {
            _client = client;
            ItemsFileType = itemsFileType;
            HeaderText = Enum.GetName(typeof(ClientItemType), itemsFileType);
            DescriptionText = helpInfo;

            UpdateListItems = ReactiveCommand.CreateFromTask<InstrumentType>(UpdateList);
            //this.WhenAnyValue(x => x.SelectedInstrumentType)
            //    .Where(x => x != null)
            //    .Select(i => ActiveItemList())
            //    .ToProperty(this, x => x.CurrentItemsList, out _currentItemsList);
            ActiveItems.Changed
                .Subscribe(x => ActiveItemList = ActiveItems.ToList());

            var canAddItem = this.WhenAnyValue(x => x.SelectedItem, x => x.ItemValue, x => x.SelectedItemDescription, 
                (selectedItem, itemValue, selectedItemDescription) => selectedItem != null && (itemValue != null || selectedItemDescription != null));
            AddItemCommand = ReactiveCommand.CreateFromTask(AddItem, canAddItem);

            DeleteRowCommand = ReactiveCommand.Create<ItemValue>((Action<ItemValue>)(x =>
            {
                ActiveItems.Remove(x);
            }));           

            /*
            * Item values combobox and textbox functionality
            */
            var itemSelected = this.WhenAnyValue(x => x.SelectedItem)
                .Where(x => x != null);

            itemSelected.Subscribe(_ =>
                {
                    SelectedItemDescription = null;
                    ItemValue = null;
                });

            //Toggle Item Descriptions combo box
            itemSelected
                .Select(x => x.ItemDescriptions.Any())
                .ToProperty(this, x => x.ShowItemDescriptions, out _showItemDescriptions);

            itemSelected
                .Select(x => x.ItemDescriptions?.ToList())
                .ToProperty(this, x => x.ItemDescriptionsList, out _itemDescriptionsList);
            
            //Toggle Item value text box
            itemSelected
                .Select(x => !x.ItemDescriptions.Any())
                .ToProperty(this, x => x.ShowItemValueTextBox, out _showItemValueTextBox);
        }

        #region Properties

        private ClientItems _clientItems;
        public ClientItems ClientItems
        {
            get { return _clientItems; }
            set { this.RaiseAndSetIfChanged(ref _clientItems, value); }
        }

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

        private ItemMetadata.ItemDescription _selectedItemDescription;
        public ItemMetadata.ItemDescription SelectedItemDescription
        {
            get { return _selectedItemDescription; }
            set { this.RaiseAndSetIfChanged(ref _selectedItemDescription, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _showItemDescriptions;
        public bool ShowItemDescriptions => _showItemDescriptions.Value;

        private readonly ObservableAsPropertyHelper<bool> _showItemValueTextBox;
        public bool ShowItemValueTextBox => _showItemValueTextBox.Value;

        private readonly ObservableAsPropertyHelper<List<ItemMetadata.ItemDescription>> _itemDescriptionsList;
        public List<ItemMetadata.ItemDescription> ItemDescriptionsList => _itemDescriptionsList.Value;

        private readonly ObservableAsPropertyHelper<List<ItemValue>> _currentItemsList;
        public List<ItemValue> CurrentItemsList => _currentItemsList.Value;

        private ReactiveList<ItemMetadata> _availableItems = new ReactiveList<ItemMetadata>();
        public ReactiveList<ItemMetadata> AvailableItems
        {
            get { return _availableItems; }
            set { this.RaiseAndSetIfChanged(ref _availableItems, value); }
        }

        private ReactiveList<ItemValue> _activeItems = new ReactiveList<ItemValue>()
        {
            ChangeTrackingEnabled = true,
        };

        public ReactiveList<ItemValue> ActiveItems
        {
            get => _activeItems;
            set => this.RaiseAndSetIfChanged(ref _activeItems, value);
        }

        private string _headerText;
        public string HeaderText
        {
            get { return _headerText; }
            set { this.RaiseAndSetIfChanged(ref _headerText, value); }
        }

        private string _descriptionText;
        public string DescriptionText
        {
            get { return _descriptionText; }
            set { this.RaiseAndSetIfChanged(ref _descriptionText, value); }
        }

        private InstrumentType _selectedInstrumentType;
        public InstrumentType SelectedInstrumentType
        {
            get { return _selectedInstrumentType; }
            set { this.RaiseAndSetIfChanged(ref _selectedInstrumentType, value); }
        }
        
        #endregion

        #region Commands

        private ReactiveCommand<InstrumentType, Unit> _updateListItems;
        public ReactiveCommand<InstrumentType, Unit> UpdateListItems
        {
            get { return _updateListItems; }
            set { this.RaiseAndSetIfChanged(ref _updateListItems, value); }
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

        #region Private Functions

        private async Task AddItem()
        {
            var value = ItemValue?.ToString() ?? SelectedItemDescription.Id.ToString();
            var itemValue = new ItemValue(SelectedItem, value);
            ActiveItems.Add(itemValue);            

            SelectedItem = null;
            SelectedItemDescription = null;
            ItemValue = null;
        }

        private async Task UpdateList(InstrumentType instrumentType)
        {
            SelectedInstrumentType = instrumentType;
            AvailableItems.Clear();
            AvailableItems.AddRange(ItemHelpers.LoadItems(instrumentType));

            using (ActiveItems.SuppressChangeNotifications())
            {
                ActiveItems.Clear();
                var clientItems = GetItemList(instrumentType, ItemsFileType);
                if (clientItems == null)
                {
                    var clientItem = new ClientItems(_client)
                    {
                        InstrumentType = instrumentType,
                        ItemFileType = ItemsFileType,
                        Items = new List<ItemValue>()
                    };
                    _client.Items.Add(clientItem);
                    
                    ActiveItems.AddRange(clientItem.Items.ToList());
                }
                else if (ActiveItemList.Any())
                {
                    ActiveItems.AddRange(ActiveItemList);
                }
            }
        }

        private ClientItems GetItemList(InstrumentType instrumentType, ClientItemType clientItemType)
        {
            return
                _client.Items.FirstOrDefault(x => x.InstrumentType == instrumentType && x.ItemFileType == clientItemType);
        }

        //private List<ItemValue> ActiveItemList()
        //{
        //    return _client.Items
        //        .FirstOrDefault(x => x.InstrumentType == SelectedInstrumentType && x.ItemFileType == ItemsFileType)?.Items.ToList();
        //}

        private List<ItemValue> ActiveItemList
        {
            get => GetItemList(SelectedInstrumentType, ItemsFileType)?.Items.OrderBy(i => i.Metadata.Number).ToList();
            set => GetItemList(SelectedInstrumentType, ItemsFileType).Items = value;
        }
        #endregion

    }
}