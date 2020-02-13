using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;

namespace Devices.Core.Interfaces
{
    public abstract class DeviceInstance
    {
        protected readonly HashSet<ItemValue> ItemValues = new HashSet<ItemValue>();
        protected readonly ConcurrentDictionary<Type, ItemGroup> GroupCache = new ConcurrentDictionary<Type, ItemGroup>();

        protected DeviceInstance(DeviceType deviceType)
        {
            DeviceType = deviceType;
        }

        #region Public Properties

        public DeviceType DeviceType { get; }

        public ICollection<ItemValue> Values => ItemValues.ToList();

        #endregion

        #region Public Methods

        public virtual TGroup ItemGroup<TGroup>() where TGroup : ItemGroup
        {
            if (GroupCache.TryGetValue(typeof(TGroup), out var cacheItem))
            {
                return (TGroup) cacheItem;
            }

            var result = DeviceType.GetGroupValues<TGroup>(Values);
            GroupCache.TryAdd(typeof(TGroup), result);
            return result;
        }

        public virtual TGroup ItemGroup<TGroup>(IEnumerable<ItemValue> values) where TGroup : ItemGroup
        {
            var v = values.ToList();

            var joined = Values.Except(v, new ItemValueComparer()).ToList();

            var result = DeviceType.GetGroupValues<TGroup>(joined.Union(v));
            return result;
        }

        public virtual TGroup CreateItemGroup<TGroup>() where TGroup : ItemGroup
        {
            var result = DeviceType.GetGroupValues<TGroup>(Values);
            return result;
        }



        public virtual void SetItemValues(IEnumerable<ItemValue> itemValues)
        {
            GroupCache.Clear();
            SetValues(itemValues);
        }

        protected abstract void SetValues(IEnumerable<ItemValue> itemValues);

        #endregion
    }
}