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

        public static Type GetMatchingItemGroupClass(this Type itemGroupType, Assembly assembly, Assembly baseAssembly = null)
        {
            if (itemGroupType.IsInterface)
            {
                var groupClass = assembly.GetTypes().FirstOrDefault(itemGroupType.IsAssignableFrom);

                if (groupClass == null && baseAssembly != null)
                    groupClass = baseAssembly.GetTypes().FirstOrDefault(itemGroupType.IsAssignableFrom);

                return groupClass;
            }

            return itemGroupType;
        }
    }
}