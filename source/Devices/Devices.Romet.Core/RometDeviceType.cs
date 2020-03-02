using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            ItemFactory = new RometItemGroupFactory();
        }

        public RometDeviceType()
        {
            Factory = new RometDeviceInstanceFactory(this);
            ItemFactory = new RometItemGroupFactory();
        }

        public override Type GetBaseItemGroupClass(Type itemGroupType)
        {
            var groupClass = GetType().Assembly.DefinedTypes.FirstOrDefault(t => t.IsSubclassOf(itemGroupType));

            if (groupClass == null)
                groupClass = base.GetBaseItemGroupClass(itemGroupType).GetTypeInfo();

            return groupClass;
        }
    }
}