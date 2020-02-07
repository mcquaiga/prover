using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;

namespace Devices.Honeywell.Core.Items.ItemGroups.Builders
{
    public class HoneywellItemGroupFactory : ItemGroupFactoryBase
    {
        public HoneywellItemGroupFactory(HoneywellDeviceType deviceType) : base (deviceType)
        {
            BasicGroupBuilder = new HoneywellItemGroupBuilder<IItemGroup>();
        }

        #region Public Methods

        public override TGroup Create<TGroup>(IEnumerable<ItemValue> values)
        {
            var builder = findGroupBuilder(typeof(TGroup));
            if (builder != null)
                return (TGroup) builder.Build(DeviceType, values);

            return (TGroup)BasicGroupBuilder.GetItemGroupInstance(typeof(TGroup), values);
        }

        protected override Assembly BaseAssembly => null;

        #endregion

        #region Private

        #endregion
    }
}