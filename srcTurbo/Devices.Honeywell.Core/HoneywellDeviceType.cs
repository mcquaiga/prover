using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Honeywell.Core.Attributes;
using Devices.Honeywell.Core.Devices;
using Devices.Honeywell.Core.ItemGroups;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;

namespace Devices.Honeywell.Core
{
    /// <summary>
    /// Defines the <see cref="HoneywellDeviceType"/>
    /// </summary>
    public class HoneywellDeviceType : IDeviceType
    {
        public virtual int AccessCode { get; set; }

        public virtual bool? CanUseIrDaPort { get; set; }

        public virtual int Id { get; set; }

        public virtual bool IsHidden { get; set; }

        public virtual ICollection<ItemMetadata> Items => _itemDefinitions.OrderBy(i => i.Number).ToList();

        public IObservable<ItemMetadata> ItemsObservable { get; }

        public virtual int? MaxBaudRate { get; set; }

        public virtual string Name { get; set; }

        public HoneywellDeviceType(IEnumerable<ItemMetadata> items)
        {
            ItemsObservable = items.ToObservable().SubscribeOn(NewThreadScheduler.Default);
            ItemsObservable
               .ToList()
               .Subscribe(x => _itemDefinitions.UnionWith(x));
        }

        [JsonConstructor]
        public HoneywellDeviceType(IEnumerable<ItemMetadata> globalItems, IEnumerable<ItemMetadata> overrideItems, IEnumerable<ItemMetadata> excludeItems)
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

        public virtual IEnumerable<ItemValue> Convert<T>(IDictionary<T, string> values) where T : struct
        {
            return ItemConvert.ToItemValues(this, (Dictionary<int, string>)values);
        }

        public virtual IDeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues)
        {
            var items = GetItemValuesByGroup<IVolumeItems>(itemValues);
            if (items.DriveRateDescription != "Rotary")
            {
                return new MechanicalDeviceInstance(this, itemValues);
            }

            return new HoneywellDeviceInstance(this, itemValues);
        }

        public virtual IEnumerable<ItemMetadata> GetItemNumbersByGroup<T>() where T : IItemsGroup
        {
            var itemType = GetMatchingItemGroupClass(typeof(T));
            var items = ItemInfoAttributeHelpers.GetItemIdentifiers(itemType);
            return GetItemMetadata(items);
        }

        public virtual T GetItemValuesByGroup<T>(IEnumerable<ItemValue> values) where T : IItemsGroup
        {
            var itemType = GetMatchingItemGroupClass(typeof(T));
            var itemGroup = (T)Activator.CreateInstance(itemType);
            itemGroup.SetValues(values);
            return itemGroup;
        }

        protected virtual Type GetMatchingItemGroupClass(Type typeInterface)
        {
            return Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => typeInterface.IsAssignableFrom(t));
        }

        private readonly HashSet<ItemMetadata> _itemDefinitions = new HashSet<ItemMetadata>();

        private IEnumerable<ItemMetadata> GetItemMetadata(IEnumerable<int> itemNumbers)
        {
            return Items.Join(itemNumbers, im => im.Number, i => i, (x, y) => x);
        }
    }
}