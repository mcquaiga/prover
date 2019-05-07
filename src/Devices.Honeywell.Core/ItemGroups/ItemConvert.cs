using Devices.Core.Interfaces;
using Devices.Core.Items;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Honeywell.Core.ItemGroups
{
    internal static class ItemConvert
    {
        public static IEnumerable<ItemValue> ToItemValues(this IDevice deviceType, IDictionary<int, string> values)
        {
            return deviceType.Items.Join(values,
                x => x.Number,
                y => y.Key,
                (im, value) => new ItemValue(im, value.Value));
        }
    }
}