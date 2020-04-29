using System;
using System.Collections.Generic;
using System.Text.Json;
using Devices.Core.Items;
using Devices.Core.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Devices.Core.Interfaces
{
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
}