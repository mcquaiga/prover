using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups
{
    public static class ItemGroupExtensions
    {

        public static Type GetMatchingItemGroupClass(this Type itemGroupType, DeviceType deviceType)
        {
            return deviceType.GetBaseItemGroupClass(itemGroupType);
        }

        public static IEnumerable<ItemMetadata> GetMatchingItemMetadata(this IEnumerable<ItemMetadata> items,
            IEnumerable<int> itemNumbers)
        {
            return items.Join(itemNumbers, im => im.Number, i => i, (x, y) => x);
        }
    }
}