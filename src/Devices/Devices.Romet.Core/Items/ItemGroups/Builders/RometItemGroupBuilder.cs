using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core;

namespace Devices.Romet.Core.Items.ItemGroups.Builders
{
    public class RometItemGroupBuilder<TGroup> : ItemGroupBuilderBase<TGroup>
        where TGroup : ItemGroup
    {
        #region Protected

        public RometItemGroupBuilder(DeviceType deviceType) : base(deviceType)
        {
        }

        protected override Assembly BaseAssembly => Assembly.GetAssembly(typeof(HoneywellDeviceType));

        #endregion
    }
}