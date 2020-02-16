using System.Collections.Generic;
using Devices.Core.Items;
using Devices.Honeywell.Core;
using Devices.Romet.Core.Items.ItemGroups.Builders;

namespace Devices.Romet.Core
{
    public class RometDeviceType : HoneywellDeviceType
    {
        public RometDeviceType(IEnumerable<ItemMetadata> items)
            : base(items)
        {
            Factory = new RometDeviceInstanceFactory(this);
            ItemFactory = new RometItemGroupFactory(this);
        }

        public RometDeviceType()
        {
            Factory = new RometDeviceInstanceFactory(this);
            ItemFactory = new RometItemGroupFactory(this);
        }
    }
}