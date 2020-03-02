using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Devices.Core.Interfaces
{
    public abstract class DeviceInstance
    {
        protected readonly ConcurrentDictionary<Type, ItemGroup> GroupCache =
            new ConcurrentDictionary<Type, ItemGroup>();

        protected readonly HashSet<ItemValue> ItemValues = new HashSet<ItemValue>();

        protected DeviceInstance(DeviceType deviceType) => DeviceType = deviceType;

        public DeviceType DeviceType { get; }

        public ICollection<ItemValue> Values => ItemValues.ToList();

        public virtual TGroup CreateItemGroup<TGroup>() where TGroup : ItemGroup =>
            DeviceType.GetGroupValues<TGroup>(Values);

        public virtual TGroup ItemGroup<TGroup>() where TGroup : ItemGroup
        {
            if (GroupCache.TryGetValue(typeof(TGroup), out var cacheItem)) return (TGroup) cacheItem;

            var result = DeviceType.GetGroupValues<TGroup>(Values);
            GroupCache.TryAdd(typeof(TGroup), result);
            return result;
        }

        public virtual TGroup ItemGroup<TGroup>(IEnumerable<ItemValue> values) where TGroup : ItemGroup
        {
            var v = values.ToList();

            var joined = Values.Except(v, new ItemValueComparer()).ToList();

            return DeviceType.GetGroupValues<TGroup>(joined.Union(v));
        }


        public virtual void SetItemValues(IEnumerable<ItemValue> itemValues)
        {
            GroupCache.Clear();
            SetValues(itemValues);
        }

        protected abstract void SetValues(IEnumerable<ItemValue> itemValues);
    }

    public static class DeviceInstanceEx
    {
        public static CompositionType Composition(this DeviceInstance device) =>
            device.ItemGroup<SiteInformationItems>().CompositionType;
    }
}