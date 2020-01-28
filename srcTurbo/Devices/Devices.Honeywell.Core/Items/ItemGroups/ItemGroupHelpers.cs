using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public static class ItemGroupHelpers
    {
        internal static Type GetMatchingItemGroupClass(Type typeInterface)
        {
            return Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(typeInterface.IsAssignableFrom);
        }

        public static T GetItemGroup<T>(IEnumerable<ItemValue> values = null) where T : IItemsGroup
        {
            var itemType = GetMatchingItemGroupClass(typeof(T));
            var itemGroup = (T) Activator.CreateInstance(itemType);

            if (values == null)
                values = new List<ItemValue>();

            var itemValues = values.ToList();
            itemGroup.SetValues(itemValues);

            return itemGroup;
        }

        public static IEnumerable<ItemMetadata> GetItemMetadata(IEnumerable<ItemMetadata> items,
            IEnumerable<int> itemNumbers)
        {
            return items.Join(itemNumbers, im => im.Number, i => i, (x, y) => x);
        }

        public static ItemMetadata GetItem(this IDeviceType deviceType, int itemNumber)
        {
            return deviceType.Items.GetItem(itemNumber);
        }
        
        public static IEnumerable<ItemValue> ToItemValues(this IDictionary<int, string> values, IDeviceType deviceType)
        {
            return deviceType.Items.Join(values,
                x => x.Number,
                y => y.Key,
                (im, value) => new ItemValue(im, value.Value));
        }
        //public T GetItemsByGroup<T>(IEnumerable<ItemValue> values) where T : IItemsGroup
        //{
        //    var results = values.Union(ItemValues, new ItemValueComparer());

        //    return GetItemValuesByGroup<T>();
        //}
    }
}