using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;

namespace Prover.Infrastructure.EntityFrameworkSqlDataAccess.Entities
{
    public class JsonDeviceInstanceConverter : JsonConverter<DeviceInstance>
    {
        private readonly DeviceRepository _deviceRepository;

        public JsonDeviceInstanceConverter(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return true;
        }

        public override DeviceInstance Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }
            
            var tempDevice = JsonSerializer.Deserialize<Device>(ref reader);
      
            var device = _deviceRepository.GetById(tempDevice.DeviceTypeId);
            return device.CreateInstance(tempDevice.Values);
        }

        public override void Write(Utf8JsonWriter writer, DeviceInstance value, JsonSerializerOptions options)
        {
            //writer.WriteStartObject();

            var device = new Device(value);
            JsonSerializer.Serialize(writer, device);
      
            //writer.WriteEndObject();
        }

        private class Device
        {
            private Device()
            {

            }

            public Device(DeviceInstance value)
            {
                DeviceTypeId = value.DeviceType.Id;
                Values = value.Values.ToItemValuesDictionary();
            }

            public Guid DeviceTypeId { get; set; }
            public Dictionary<string, string> Values { get; set; }
        }
    }
}