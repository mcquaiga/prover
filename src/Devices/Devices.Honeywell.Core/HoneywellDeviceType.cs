using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items.ItemGroups.Builders;
using Newtonsoft.Json;

namespace Devices.Honeywell.Core
{
    /// <summary>
    ///     Defines the <see cref="HoneywellDeviceType" />
    /// </summary>
    public class HoneywellDeviceType : DeviceType
    {
        [JsonConstructor]
        public HoneywellDeviceType(IEnumerable<ItemMetadata> items) : base(items)
        {
            Factory = new HoneywellDeviceInstanceFactory(this);
            ItemFactory = new HoneywellItemGroupFactory();

            Items = Items.OrderBy(i => i.Number).ToList();
        }

        public HoneywellDeviceType()
        {
            Factory = new HoneywellDeviceInstanceFactory(this);
            ItemFactory = new HoneywellItemGroupFactory();
        }

        #region Public Properties

        public virtual string AccessCode { get; set; }

        #endregion

        #region Public Methods

        #region Methods

        public override IEnumerable<ItemMetadata> GetItemMetadata<T>()
        {
            var items = GetItemNumbersByGroup<T>();
            return Items.GetMatchingItemMetadata(items);
        }

        public override TGroup GetGroupValues<TGroup>(IEnumerable<ItemValue> itemValues)
        {
            return (TGroup) ItemFactory.Create<TGroup>(this, itemValues);
        }

        public override ItemGroup GetGroupValues(IEnumerable<ItemValue> itemValues, Type groupType)
            => ItemFactory.Create(this, itemValues, groupType);

        public override Type GetBaseItemGroupClass(Type itemGroupType)
        {
            var groupClass = GetType().Assembly.DefinedTypes.FirstOrDefault(t => t.IsSubclassOf(itemGroupType));

            if (groupClass == null)
            {
                groupClass = typeof(HoneywellDeviceType).Assembly.DefinedTypes.FirstOrDefault(t => t.IsSubclassOf(itemGroupType));
            }

            return groupClass;
        }

        #endregion

        #endregion
    }
}