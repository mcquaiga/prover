using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace CrWall.Screens.Items
{
    public class ItemViewModel : ViewModelBase
    {
        public ItemViewModel(InstrumentType instrumentType, ItemValue itemValue = null)
        {
            Items = ItemHelpers.LoadItems(instrumentType);

            ItemStrings = Items.Select(x => x.LongDescription).ToList();
        }

        public ItemViewModel(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
            Items = ItemHelpers.LoadItems(Instruments.MiniAt);

            ItemStrings = Items.Select(x => x.LongDescription).ToList();
        }

        private IEnumerable<string> _itemStrings;
        public IEnumerable<string> ItemStrings
        {
            get { return _itemStrings; }
            set { this.RaiseAndSetIfChanged(ref _itemStrings, value); }
        }

        private IEnumerable<ItemMetadata> _items;
        public IEnumerable<ItemMetadata> Items
        {
            get { return _items; }
            set { this.RaiseAndSetIfChanged(ref _items, value); }
        }

        private ItemMetadata _selectedItem;
        public ItemMetadata SelectedItem
        {
            get { return _selectedItem; }
            set { this.RaiseAndSetIfChanged(ref _selectedItem, value); }
        }
    }
}
