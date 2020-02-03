using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;

namespace Devices.Core.Interfaces
{
    public abstract class DeviceType
    {
        protected readonly HashSet<ItemMetadata> ItemDefinitions = new HashSet<ItemMetadata>();

        #region Public Properties

        public virtual bool? CanUseIrDaPort { get; set; }
        public virtual bool IsHidden { get; set; }
        public virtual ICollection<ItemMetadata> Items => ItemDefinitions.ToList();
        public virtual int? MaxBaudRate { get; set; }
        public virtual string Name { get; set; }
        public IDeviceInstanceFactory Factory { get; protected set; }
        #endregion

        #region Public Methods

        public virtual DeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null)
        {
            return Factory.CreateInstance(itemValues);
        }

        public abstract IEnumerable<ItemMetadata> GetItemsByGroup<T>() where T : IItemGroup;
        #endregion
    }

    //public abstract class DeviceType
    //{
    //    public bool? CanUseIrDaPort { get; }
    //    public bool IsHidden { get; }
    //    public ICollection<ItemMetadata> Items { get; }
    //    public int? MaxBaudRate { get; }
    //    public string Name { get; }
    //    public IDeviceInstanceFactory InstanceFactory { get; protected set; }
    //    public abstract IEnumerable<ItemMetadata> GetItemMetadataByGroup<T>() where T : IItemGroup;
    //}
}