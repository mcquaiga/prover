using System.Collections.Generic;
using Devices.Core.Items;
using Devices.Honeywell.Core;
using Newtonsoft.Json;

namespace Devices.Romet.Core
{
    public interface IRometDeviceType : IHoneywellDeviceType
    {

    }

    public class RometDeviceType : HoneywellDeviceType, IRometDeviceType
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