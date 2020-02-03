using System;
using System.Collections.Generic;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Core.Interfaces
{
    public abstract class DeviceInstance
    {
        protected DeviceInstance(DeviceType deviceType)
        {
            DeviceType = deviceType;
        }

        #region Public Properties

        public DeviceType DeviceType { get; }
        public HashSet<ItemValue> ItemValues { get; protected set; } = new HashSet<ItemValue>();

        public ISiteInformationItems SiteInfo { get; protected set; }
        public virtual Dictionary<Type, IItemGroup> Attributes { get; } = new Dictionary<Type, IItemGroup>();

        #endregion

        #region Public Methods

        public abstract T GetItemsByGroup<T>(IEnumerable<ItemValue> values) where T : IItemGroup;

        public abstract void SetItemGroups(IEnumerable<ItemValue> itemValues);

        public virtual void AddAttribute<TGroup>(TGroup itemGroup) where TGroup : ItemGroup
        {
            Attributes.Add(typeof(TGroup), itemGroup);
        }

        public virtual TGroup FindAttribute<TGroup>() where TGroup : IItemGroup
        {
            if (Attributes.TryGetValue(typeof(TGroup), out var result))
            {
                return (TGroup)result;
            }

            return default(TGroup);
        }

        #endregion
    }
}