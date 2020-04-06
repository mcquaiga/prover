using System.Collections.Generic;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;

namespace Devices.Honeywell.Core.Items.ItemGroups.Builders
{
    public class HoneywellItemGroupFactory : ItemGroupFactoryBase
    {
        public HoneywellItemGroupFactory()
        {
            BasicGroupBuilder = new HoneywellItemGroupBuilder<ItemGroup>();
        }

        #region Public Methods

        public override TGroup Create<TGroup>(DeviceType deviceType, IEnumerable<ItemValue> values)
        {
            //var builder = findGroupBuilder(typeof(TGroup), deviceType);
            //if (builder != null)
            //    return (TGroup) builder.Build(deviceType, values);

            return (TGroup)BasicGroupBuilder.GetItemGroupInstance(typeof(TGroup), values, deviceType);
        }

        protected override Assembly BaseAssembly => null;

        #endregion

        #region Private

        #endregion
    }
}