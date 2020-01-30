using System.Collections.Generic;
using Devices.Core.Items;
using Devices.Honeywell.Core;
using Newtonsoft.Json;

namespace Devices.Romet.Core
{
    public class RometDeviceType : HoneywellDeviceType
    {
        public RometDeviceType(IEnumerable<ItemMetadata> items)
            : base(items)
        {
        }

        [JsonConstructor]
        public RometDeviceType(IEnumerable<ItemMetadata> globalItems, IEnumerable<ItemMetadata> overrideItems, IEnumerable<ItemMetadata> excludeItems) 
            : base(globalItems, overrideItems, excludeItems)
        {
        }
    }
}