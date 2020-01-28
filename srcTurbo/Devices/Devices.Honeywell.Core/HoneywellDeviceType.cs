using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Honeywell.Core.Attributes;
using Devices.Honeywell.Core.Items.ItemGroups;
using Newtonsoft.Json;

namespace Devices.Honeywell.Core
{
    /// <summary>
    ///     Defines the <see cref="HoneywellDeviceType" />
    /// </summary>
    public class HoneywellDeviceType : IDeviceType
    {
        public HoneywellDeviceType(IEnumerable<ItemMetadata> items)
        {
            ItemsObservable = items.ToObservable().SubscribeOn(NewThreadScheduler.Default);
            ItemsObservable
                .ToList()
                .Subscribe(x => _itemDefinitions.UnionWith(x));
        }

        [JsonConstructor]
        public HoneywellDeviceType(IEnumerable<ItemMetadata> globalItems, IEnumerable<ItemMetadata> overrideItems,
            IEnumerable<ItemMetadata> excludeItems)
        {
            globalItems = globalItems ?? new List<ItemMetadata>();
            overrideItems = overrideItems ?? new List<ItemMetadata>();
            excludeItems = excludeItems ?? new List<ItemMetadata>();

            ItemsObservable = globalItems.Concat(overrideItems)
                .Where(item => excludeItems.All(x => x.Number != item.Number))
                .GroupBy(item => item.Number)
                .Select(group => group.Aggregate((_, next) => next))
                .OrderBy(i => i.Number)
                .ToObservable()
                .SubscribeOn(NewThreadScheduler.Default);

            ItemsObservable
                .ToList()
                .Subscribe(x => _itemDefinitions.UnionWith(x));
        }

        private readonly HashSet<ItemMetadata> _itemDefinitions = new HashSet<ItemMetadata>();

        public virtual int AccessCode { get; set; }

        public virtual int Id { get; set; }

        public IObservable<ItemMetadata> ItemsObservable { get; }

        public virtual bool? CanUseIrDaPort { get; set; }

        public virtual bool IsHidden { get; set; }

        public virtual ICollection<ItemMetadata> Items => _itemDefinitions.OrderBy(i => i.Number).ToList();

        public virtual int? MaxBaudRate { get; set; }

        public virtual string Name { get; set; }

        public IDeviceInstance CreateDeviceInstance(IEnumerable<ItemValue> itemValues)
        {
            return this.CreateInstance(itemValues);
        }

        public IDeviceInstance CreateDeviceInstance(IDictionary<int, string> itemValuesDictionary)
        {
            return this.CreateInstance(itemValuesDictionary);
        }
        
        public virtual IEnumerable<ItemMetadata> GetItemMetadataByGroup<T>() where T : IItemsGroup
        {
            var itemType = ItemGroupHelpers.GetMatchingItemGroupClass(typeof(T));
            var items = ItemInfoAttributeHelpers.GetItemIdentifiers(itemType);
            return ItemGroupHelpers.GetItemMetadata(Items, items);
        }
    }
}