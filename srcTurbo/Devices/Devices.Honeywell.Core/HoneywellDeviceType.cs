using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items;
using Newtonsoft.Json;

namespace Devices.Honeywell.Core
{
    /// <summary>
    ///     Defines the <see cref="HoneywellDeviceType" />
    /// </summary>
    public class HoneywellDeviceType : DeviceType
    {
        [JsonConstructor]
        public HoneywellDeviceType(IEnumerable<ItemMetadata> items)
        {
            Factory = HoneywellDeviceInstanceFactory.Find(this);

            ItemDefinitions.UnionWith(items);
        }

        #region Public Properties

        public virtual string AccessCode { get; set; }

        public virtual int Id { get; set; }

        public override ICollection<ItemMetadata> Items => ItemDefinitions.OrderBy(i => i.Number).ToList();

        public IObservable<ItemMetadata> ItemsObservable => Items.ToObservable();

        #endregion

        #region Public Methods

        #region Methods

        public override IEnumerable<ItemMetadata> GetItemsByGroup<T>()
        {
            var items = ItemMetadataExtensions.GetItemNumbersByGroup<T>();
            return Items.GetMatchingItemMetadata(items);
        }

        #endregion

        #endregion
    }
}