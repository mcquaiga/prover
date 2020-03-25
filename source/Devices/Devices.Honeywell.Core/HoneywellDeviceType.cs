using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core.Items.ItemGroups.Builders;
using Newtonsoft.Json;

namespace Devices.Honeywell.Core
{
    /// <summary>
    ///     Defines the <see cref="HoneywellDeviceType" />
    /// </summary>
    public class HoneywellDeviceType : DeviceType
    {
        [JsonConstructor]
        public HoneywellDeviceType(IEnumerable<ItemMetadata> items) : base(items)
        {
            Items = Items.OrderBy(i => i.Number).ToList();
        }

        public HoneywellDeviceType()
        {
        }

        protected override ItemGroupFactoryBase ItemFactory { get; } = new HoneywellItemGroupFactory();
        public override IDeviceInstanceFactory Factory => new HoneywellDeviceInstanceFactory(this);

        public virtual string AccessCode { get; set; }

        public override Type GetBaseItemGroupClass(Type itemGroupType)
        {
            var groupClass = GetType().Assembly.DefinedTypes.FirstOrDefault(t => t.IsSubclassOf(itemGroupType));

            if (groupClass == null)
                groupClass =
                    typeof(HoneywellDeviceType).Assembly.DefinedTypes.FirstOrDefault(t =>
                        t.IsSubclassOf(itemGroupType));

            return groupClass;
        }

        public override TGroup GetGroupValues<TGroup>(IEnumerable<ItemValue> itemValues) =>
            ItemFactory.Create<TGroup>(this, itemValues);

        public override ItemGroup GetGroupValues(IEnumerable<ItemValue> itemValues, Type groupType)
            => ItemFactory.Create(this, itemValues, groupType);

        public override IEnumerable<ItemMetadata> GetItemMetadata<T>()
        {
            var items = GetItemNumbersByGroup<T>();
            return Items.GetMatchingItemMetadata(items);
        }
    }
}