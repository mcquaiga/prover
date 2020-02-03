using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups
{
    public static class ItemGroupExtensions
    {
        public static IEnumerable<ItemMetadata> GetMatchingItemMetadata(this IEnumerable<ItemMetadata> items,
            IEnumerable<int> itemNumbers)
        {
            return items.Join(itemNumbers, im => im.Number, i => i, (x, y) => x);
        }
    }
}