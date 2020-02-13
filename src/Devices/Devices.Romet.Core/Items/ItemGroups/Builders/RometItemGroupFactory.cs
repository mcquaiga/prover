﻿using System.Reflection;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core.Items.ItemGroups.Builders;

namespace Devices.Romet.Core.Items.ItemGroups.Builders
{
    public class RometItemGroupFactory : ItemGroupFactoryBase
    {
        public RometItemGroupFactory(RometDeviceType deviceType) : base(deviceType)
        {
            BasicGroupBuilder = new RometItemGroupBuilder<ItemGroup>(deviceType);
        }

        #region Protected

        protected override Assembly BaseAssembly => Assembly.GetAssembly(typeof(HoneywellItemGroupFactory));

        #endregion
    }
}