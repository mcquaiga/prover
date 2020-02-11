using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups.Builders;

namespace Devices.Core.Interfaces
{
    public abstract class DeviceType
    {
        protected readonly HashSet<ItemMetadata> ItemDefinitions = new HashSet<ItemMetadata>();
        protected ItemGroupFactoryBase ItemFactory;
        protected DeviceType(IEnumerable<ItemMetadata> itemDefinitions)
        {
            ItemDefinitions.UnionWith(itemDefinitions);
        }

        #region Public Properties
        public virtual Guid Id { get; set; }
        public virtual bool? CanUseIrDaPort { get; set; }
        public virtual bool IsHidden { get; set; }
        public virtual ICollection<ItemMetadata> Items => ItemDefinitions.ToList();
        public virtual int? MaxBaudRate { get; set; }
        public virtual string Name { get; set; }
        public IDeviceInstanceFactory Factory { get; protected set; }
        #endregion

        #region Public Methods

        //public virtual DeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null)
        //{
        //    return Factory?.CreateInstance(itemValues);
        //}

        //public virtual DeviceInstance CreateInstance(IDictionary<int, string> itemValuesDictionary)
        //{
        //    var items = ToItemValuesEnumerable(this, itemValuesDictionary);
        //    return CreateInstance(items);
        //}

        public abstract IEnumerable<ItemMetadata> GetItemMetadata<T>() where T : IItemGroup;

        public abstract TGroup GetGroupValues<TGroup>(IEnumerable<ItemValue> itemValues) where TGroup : IItemGroup;

        #endregion

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