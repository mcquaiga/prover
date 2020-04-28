using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prover.Shared;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Devices.Core.Interfaces
{
    //public class DeviceInstanceSerializer : JsonSerializer
    //{
    //    override
    //}


    public class DeviceInstanceJsonConverter : JsonConverter<DeviceInstance>
    {
        protected readonly IDeviceRepository DeviceRepository;

        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
                PropertyNameCaseInsensitive = true
        };

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
                Formatting = Formatting.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public DeviceInstanceJsonConverter()
        {
            if (Repository.DeviceRepository.Instance == null) 
                throw new NullReferenceException("Device repository has not been initialized");
            DeviceRepository = Repository.DeviceRepository.Instance;
        }

        public DeviceInstanceJsonConverter(IDeviceRepository deviceRepository) => DeviceRepository = deviceRepository;

        /// <inheritdoc />
        public DeviceInstance Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = reader.GetString();
            var device = System.Text.Json.JsonSerializer.Deserialize<DeviceSerialized>(json, _options);
            var deviceType = DeviceRepository.GetById(device.DeviceTypeId);
            return deviceType.CreateInstance(device.Values);
        }

        /// <inheritdoc />
        public override DeviceInstance ReadJson(JsonReader reader, Type objectType, DeviceInstance existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JToken.Load(reader);
            
            var tempDevice = jObject.ToObject<DeviceSerialized>();
       
            var deviceType = DeviceRepository.GetById(tempDevice.DeviceTypeId);
            return deviceType.CreateInstance(tempDevice.Values);
        }

        /// <inheritdoc />
        public void Write(Utf8JsonWriter writer, DeviceInstance value, JsonSerializerOptions options)
        {
            var device = DeviceSerialized.Create(value);
            writer.WriteStringValue(System.Text.Json.JsonSerializer.Serialize(device, _options));
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, DeviceInstance value, JsonSerializer serializer)
        {
            var device = DeviceSerialized.Create(value);
            
            serializer.Serialize(writer, device);
        }

        #region Nested type: Device

        protected class DeviceSerialized
        {
            private static IDeviceRepository _deviceRepo;

            static DeviceSerialized()
            {
                _deviceRepo = Repository.DeviceRepository.Instance;
            }

            public DeviceSerialized()
            {
            }

            [JsonConstructor]
            public DeviceSerialized(Guid deviceTypeId, Dictionary<string, string> values)
            {
                DeviceTypeId = deviceTypeId;
                Values = values;
            }

            private DeviceSerialized(DeviceInstance value)
            {
                DeviceTypeId = value.DeviceType.Id;
                Values = value.Values.ToItemValuesDictionary();
            }

            //[JsonIgnore]
            public DeviceType GetDeviceType() => _deviceRepo.GetById(DeviceTypeId);

            public Guid DeviceTypeId { get; }
            public Dictionary<string, string> Values { get; }

            public static DeviceSerialized Create(DeviceInstance instance) => new DeviceSerialized(instance);

            public static DeviceSerialized Create(string deviceTypeId, string values)
            {
                if (!Guid.TryParse(deviceTypeId, out var id))
                    throw new ArgumentException(nameof(deviceTypeId));
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(values);
                return new DeviceSerialized(id, dict);
            }

            public static DeviceSerialized Create(Guid deviceTypeId, Dictionary<string, string> values) => new DeviceSerialized(deviceTypeId, values);
        }

        #endregion
    }

    //public class DeviceInstanceJsonConverterSystemJson : DeviceInstanceJsonConverterBase<DeviceInstance>
    //{

    //}
    // [System.Text.Json.Serialization.JsonConverter(typeof(DeviceInstanceJsonConverter))]
    [JsonConverter(typeof(DeviceInstanceJsonConverter))]
    public abstract class DeviceInstance
    {
        protected readonly ConcurrentDictionary<Type, ItemGroup> GroupCache = new ConcurrentDictionary<Type, ItemGroup>();

        protected readonly HashSet<ItemValue> ItemValues = new HashSet<ItemValue>();

        protected DeviceInstance(DeviceType deviceType)
        {
            DeviceType = deviceType;
            Items = new DeviceItems(this);
        }

        public DeviceType DeviceType { get; }

        public DeviceItems Items { get; }

        public VolumeInputType DriveType => Items.Volume.VolumeInputType;

        public ICollection<ItemValue> Values => ItemValues.ToList();

        public void ClearCache()
        {
            GroupCache.Clear();
        }

        public virtual TGroup CreateItemGroup<TGroup>() where TGroup : ItemGroup => DeviceType.GetGroup<TGroup>(Values);
 
        public virtual TGroup CreateItemGroup<TGroup>(IEnumerable<ItemValue> values) where TGroup : ItemGroup
        {

            var joined = values?.Union(Values) // new ItemValueComparer()
                                .ToList();

            return DeviceType.GetGroup<TGroup>(joined ?? Values);
        }

        public virtual TGroup ItemGroup<TGroup>() where TGroup : ItemGroup
        {
            if (GroupCache.TryGetValue(typeof(TGroup), out var cacheItem)) return (TGroup) cacheItem;
            var result = DeviceType.GetGroup<TGroup>(Values);
            GroupCache.TryAdd(typeof(TGroup), result);
            return result;
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