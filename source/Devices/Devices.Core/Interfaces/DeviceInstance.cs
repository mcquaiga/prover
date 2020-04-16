using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Devices.Core.Interfaces
{
    public class DeviceInstanceJsonConverter : JsonConverter<DeviceInstance>
    {
        private JsonSerializerOptions _options = new JsonSerializerOptions()
        {
                PropertyNameCaseInsensitive = true
        };

        private readonly IDeviceRepository _deviceRepository;

        public DeviceInstanceJsonConverter()
        {
            if (DeviceRepository.Instance == null) throw new NullReferenceException("Device repository has not been initialized");

            _deviceRepository = DeviceRepository.Instance;
        }

        public DeviceInstanceJsonConverter(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        /// <inheritdoc />
        public override DeviceInstance Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            var json = reader.GetString();
            var device = JsonSerializer.Deserialize<Device>(json, _options);
            
            var deviceType = _deviceRepository.GetById(device.DeviceTypeId);
            return deviceType.CreateInstance(device.Values);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DeviceInstance value, JsonSerializerOptions options)
        {
            var device = Device.Create(value);
            writer.WriteStringValue(JsonSerializer.Serialize(device, _options));
        }


        private class Device
        {
            internal static Device Create(DeviceInstance instance)
            {
                return new Device(instance);
            }

            public Device()
            {
            }


            private Device(Guid deviceTypeId, Dictionary<string, string> values)
            {
                DeviceTypeId = deviceTypeId;
                Values = values;
            }

            private Device(DeviceInstance value)
            {
                DeviceTypeId = value.DeviceType.Id;
                Values = value.Values.ToItemValuesDictionary();
            }

            public Guid DeviceTypeId { get; set; }
            public Dictionary<string, string> Values { get; set; }
        }
    }


    [JsonConverter(typeof(DeviceInstanceJsonConverter))]
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