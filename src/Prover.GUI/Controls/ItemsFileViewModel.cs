using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Prover.CommProtocol.Common.Items;
using ReactiveUI;
using System.Reactive.Linq;
using Prover.Core.Models.Clients;

namespace Prover.GUI.Controls
{
    public class ItemsFileViewModel : ReactiveScreen
    {
        private readonly Client _client;

        public ItemsFileViewModel(Client client)
        {
            _client = client;        

            var canAddItem = this.WhenAnyValue(x => x.SelectedItem, x => x.ItemValue, x => x.SelectedItemDescription, 
                (selectedItem, itemValue, selectedItemDescription) => selectedItem != null && (itemValue != null || selectedItemDescription != null));

            AddItemCommand = ReactiveCommand.CreateFromTask(AddItem, canAddItem);

            DeleteRowCommand = ReactiveCommand.Create<ItemValue>(x =>
            {
                ClientItems.Items.Remove(x);
            });           

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

        private ReactiveList<ItemValue> _items = new ReactiveList<ItemValue>();
        public ReactiveList<ItemValue> Items
        {
            get { return _items; }
            set { this.RaiseAndSetIfChanged(ref _items, value); }
        }

        #endregion

        private async Task AddItem()
        {
            var value = ItemValue?.ToString() ?? SelectedItemDescription.Id.ToString();
            var itemValue = new ItemValue(SelectedItem, value);
            ClientItems.Items.Add(itemValue);           

            SelectedItem = null;
            SelectedItemDescription = null;
            ItemValue = null;
        }      
    }
}