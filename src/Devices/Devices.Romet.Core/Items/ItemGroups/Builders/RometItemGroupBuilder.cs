using System.Reflection;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core.Items.ItemGroups.Builders;

namespace Devices.Romet.Core.Items.ItemGroups.Builders
{
    public class RometItemGroupBuilder<TGroup> : ItemGroupBuilderBase<TGroup>
        where TGroup : IItemGroup
    {
        #region Protected

        protected override Assembly BaseAssembly => Assembly.GetAssembly(typeof(HoneywellItemGroupBuilder<>));

        #endregion
    }
}