using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System;
using System.Collections.Generic;

namespace Devices.Romet.Core
{
    public class RometDevice : IDeviceType
    {
        public bool? CanUseIrDaPort => throw new NotImplementedException();

        public bool IsHidden => throw new NotImplementedException();

        public ICollection<ItemMetadata> Items => throw new NotImplementedException();

        public int? MaxBaudRate => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public IEnumerable<ItemValue> Convert<T>(IDictionary<T, string> values) where T : struct
        {
            throw new NotImplementedException();
        }

        public IDeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemMetadata> GetItemNumbersByGroup<T>() where T : IItemsGroup
        {
            throw new NotImplementedException();
        }

        public T GetItemValuesByGroup<T>(IEnumerable<ItemValue> values) where T : IItemsGroup
        {
            throw new NotImplementedException();
        }
    }
}