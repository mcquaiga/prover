using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Items.ItemGroups.Builders;

namespace Devices.Romet.Core.Items.ItemGroups.Builders
{
    public class RometItemGroupBuilder<TGroup> : ItemGroupBuilderBase<TGroup>, IItemGroupBuilder<TGroup>
        where TGroup : IItemGroup
    {
        public RometItemGroupBuilder(DeviceType deviceType) : base(deviceType)
        {
        }
    }
}
