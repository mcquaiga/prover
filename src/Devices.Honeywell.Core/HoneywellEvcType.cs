using Devices.Core.Interfaces;
using Devices.Core.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Devices.Honeywell.Core
{
    public interface IHoneywellDeviceType : IDeviceType
    {
        int AccessCode { get; set; }

        int Id { get; set; }

        IObservable<ItemMetadata> ItemsObservable { get; }
    }

    /// <summary>
    /// Defines the <see cref="HoneywellDeviceType"/>
    /// </summary>
    public class HoneywellDeviceType : IHoneywellDeviceType
    {
        private readonly HashSet<ItemMetadata> _itemDefinitions = new HashSet<ItemMetadata>();

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

        public virtual int AccessCode { get; set; }

        public virtual bool? CanUseIrDaPort { get; set; }

        public ICollection<ItemMetadata> Definitions => _itemDefinitions.OrderBy(i => i.Number).ToList();

        public virtual int Id { get; set; }

        public virtual bool IsHidden { get; set; } = false;

        public virtual string ItemFilePath { get; set; }

        public IObservable<ItemMetadata> ItemsObservable { get; }

        public virtual int? MaxBaudRate { get; set; }

        public virtual string Name { get; set; }

        public IDevice CreateInstance(Dictionary<string, string> itemValues)
        {
            return new HoneywellDevice(this, itemValues);
        }

        public IDevice CreateInstance(IEnumerable<ItemValue> itemValues)
        {
            throw new HoneywellDevice(this, ;
        }
    }
}