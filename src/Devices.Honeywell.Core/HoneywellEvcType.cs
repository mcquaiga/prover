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
        #region Properties

        int AccessCode { get; set; }

        int Id { get; set; }

        IObservable<ItemMetadata> ItemsObservable { get; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="HoneywellDeviceType"/>
    /// </summary>
    public class HoneywellDeviceType : IHoneywellDeviceType
    {
        #region Constructors

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

        #endregion

        #region Fields

        private readonly HashSet<ItemMetadata> _itemDefinitions = new HashSet<ItemMetadata>();

        #endregion

        #region Properties

        public virtual int AccessCode { get; set; }

        public virtual bool? CanUseIrDaPort { get; set; }

        public ICollection<ItemMetadata> Definitions => _itemDefinitions.OrderBy(i => i.Number).ToList();

        public virtual int Id { get; set; }

        public virtual bool IsHidden { get; set; } = false;

        public virtual string ItemFilePath { get; set; }

        public IObservable<ItemMetadata> ItemsObservable { get; }

        public virtual int? MaxBaudRate { get; set; }

        public virtual string Name { get; set; }

        #endregion

        #region Methods

        public IDevice CreateInstance()
        {
            throw new NotImplementedException();
        }

        public IDevice CreateInstance(Dictionary<string, string> itemValues)
        {
            return new HoneywellDevice(this, itemValues);
        }

        #endregion
    }
}