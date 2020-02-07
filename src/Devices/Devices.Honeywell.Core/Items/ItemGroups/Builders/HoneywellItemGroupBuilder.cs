using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;

namespace Devices.Honeywell.Core.Items.ItemGroups.Builders
{
    public class HoneywellItemGroupBuilder<TGroup> : ItemGroupBuilderBase<TGroup>
        where TGroup : IItemGroup
    {
    }
}