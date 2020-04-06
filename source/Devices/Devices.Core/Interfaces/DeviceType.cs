using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;

namespace Devices.Core.Interfaces
{
    public abstract class DeviceType
    {
        protected DeviceType(IEnumerable<ItemMetadata> itemDefinitions) =>
            Items = new List<ItemMetadata>(itemDefinitions);

        protected DeviceType() => Items = new List<ItemMetadata>();
        //protected readonly HashSet<ItemMetadata> ItemDefinitions = new HashSet<ItemMetadata>();

        protected virtual ItemGroupFactoryBase ItemFactory { get; }

        public Guid Id { get; set; }
        public virtual bool? CanUseIrDaPort { get; set; }
        public virtual bool IsHidden { get; set; }
        public ICollection<ItemMetadata> Items { get; protected set; }
        public virtual int? MaxBaudRate { get; set; }
        public virtual string Name { get; set; }

        public virtual IDeviceInstanceFactory Factory { get; protected set; }

        public abstract Type GetBaseItemGroupClass(Type itemGroupType);

        public abstract TGroup GetGroupValues<TGroup>(IEnumerable<ItemValue> itemValues) where TGroup : ItemGroup;

        public abstract ItemGroup GetGroupValues(IEnumerable<ItemValue> itemValues, Type groupType);

        public abstract IEnumerable<ItemMetadata> GetItemMetadata<T>() where T : ItemGroup;

        public IEnumerable<int> GetItemNumbersByGroup<T>()
        {
            var itemType = GetBaseItemGroupClass(typeof(T));
            return ItemInfoAttributeHelpers.GetItemIdentifiers(itemType);
        }


        public override string ToString() => Name;

        private static IEnumerable<ItemValue> ToItemValuesEnumerable(DeviceType deviceType,
            IDictionary<int, string> itemValuesDictionary)
        {
            return deviceType.Items.Join(itemValuesDictionary,
                x => x.Number,
                y => y.Key,
                (im, value) => ItemValue.Create(im, value.Value));
        }
    }
}