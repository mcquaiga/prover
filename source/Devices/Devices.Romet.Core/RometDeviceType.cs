using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core;
using Devices.Romet.Core.Items.ItemGroups.Builders;

namespace Devices.Romet.Core
{
    public class RometDeviceType : HoneywellDeviceType
    {
        public RometDeviceType(IEnumerable<ItemMetadata> items)
            : base(items)
        {
        }

        private RometDeviceType()
        {
        }

        protected override ItemGroupFactoryBase ItemFactory { get; } = new RometItemGroupFactory();
        public override IDeviceInstanceFactory Factory => new RometDeviceInstanceFactory(this);

        public override Type GetBaseItemGroupClass(Type itemGroupType)
        {
            var groupClass = GetType().Assembly.DefinedTypes.FirstOrDefault(t => t.IsSubclassOf(itemGroupType));

            if (groupClass == null)
                groupClass = base.GetBaseItemGroupClass(itemGroupType).GetTypeInfo();

            return groupClass;
        }
    }
}