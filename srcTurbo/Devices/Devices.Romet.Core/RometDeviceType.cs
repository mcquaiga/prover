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
            Factory = new RometDeviceInstanceFactory(this);
        }
    }
}