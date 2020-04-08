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

        protected DeviceInstance(DeviceType deviceType)
        {
            DeviceType = deviceType;
            Items = new DeviceItems(this);
        }

        public DeviceType DeviceType { get; }

        public DeviceItems Items { get; }

        public ICollection<ItemValue> Values => ItemValues.ToList();

        public virtual TGroup CreateItemGroup<TGroup>() where TGroup : ItemGroup =>
            DeviceType.GetGroup<TGroup>(Values);

        public virtual TGroup ItemGroup<TGroup>() where TGroup : ItemGroup
        {
            if (GroupCache.TryGetValue(typeof(TGroup), out var cacheItem)) return (TGroup) cacheItem;

            var result = DeviceType.GetGroup<TGroup>(Values);
            GroupCache.TryAdd(typeof(TGroup), result);
            return result;
        }

        public virtual TGroup CreateItemGroup<TGroup>(IEnumerable<ItemValue> values) where TGroup : ItemGroup
        {
            var joined = values.Union(Values, new ItemValueComparer()).ToList();

            return DeviceType.GetGroup<TGroup>(joined);
        }

        public void ClearCache()
        {
            GroupCache.Clear();
        }
   
        public virtual void SetItemValues(IEnumerable<ItemValue> itemValues)
        {
            ClearCache();
            SetValues(itemValues);
        }

        protected abstract void SetValues(IEnumerable<ItemValue> itemValues);
    }

    public class DeviceItems
    {
        private readonly DeviceInstance _device;
        public DeviceItems(DeviceInstance device) => _device = device;

        public SiteInformationItems SiteInfo => _device.ItemGroup<SiteInformationItems>();
        public PressureItems Pressure => _device.ItemGroup<PressureItems>();
        public TemperatureItems Temperature => _device.ItemGroup<TemperatureItems>();
        public SuperFactorItems SuperFactor => _device.ItemGroup<SuperFactorItems>();
        public PulseOutputItems PulseOutput => _device.ItemGroup<PulseOutputItems>();
        public VolumeItems Volume => _device.ItemGroup<VolumeItems>();
    }


}