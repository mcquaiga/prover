using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Core
{
    public abstract class DeviceTypeBase : IDeviceType
    {
        public virtual int AccessCode { get; set; }

        public virtual bool? CanUseIrDaPort { get; set; }

        public virtual int Id { get; set; }

        public virtual bool IsHidden { get; set; }

        public virtual ICollection<ItemMetadata> Items { get; set; }

        public IObservable<ItemMetadata> ItemsObservable { get; }

        public virtual int? MaxBaudRate { get; set; }

        public virtual string Name { get; set; }

        public abstract IEnumerable<ItemValue> Convert<T>(IDictionary<T, string> values) where T : struct;

        public abstract IDeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues);

        public abstract IEnumerable<ItemMetadata> GetItemNumbersByGroup<T>() where T : IItemsGroup;

        public abstract T GetItemValuesByGroup<T>(IEnumerable<ItemValue> values) where T : IItemsGroup;
    }
}