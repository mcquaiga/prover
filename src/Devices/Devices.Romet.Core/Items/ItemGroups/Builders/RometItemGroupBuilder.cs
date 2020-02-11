using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Items.ItemGroups.Builders;

namespace Devices.Romet.Core.Items.ItemGroups.Builders
{
    public class RometItemGroupBuilder<TGroup> : ItemGroupBuilderBase<TGroup>
        where TGroup : IItemGroup
    {
        #region Protected

        public RometItemGroupBuilder(DeviceType deviceType) : base(deviceType)
        {
        }

        protected override Assembly BaseAssembly => Assembly.GetAssembly(typeof(HoneywellDeviceType));

        #endregion
    }
}