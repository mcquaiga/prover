using System;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;

namespace Prover.Storage.MongoDb
{
    internal class DeviceInstanceBsonSerializer : DeviceInstanceJsonConverter, IBsonSerializer<DeviceInstance>
    {
        /// <inheritdoc />

        /// <inheritdoc />
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => Deserialize(context, args);

        /// <inheritdoc />
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DeviceInstance value)
        {
            var device = DeviceSerialized.Create(value);
            var json = JsonConvert.SerializeObject(device);
            context.Writer.WriteString(json);
        }

        /// <inheritdoc />
        public DeviceInstance Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var json = context.Reader.ReadString();
            var device = BsonSerializer.Deserialize<DeviceSerialized>(json);
            
            return device.GetDeviceType().CreateInstance(device.Values);
        }

        /// <inheritdoc />
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, (DeviceInstance)value);
        }

        /// <inheritdoc />
        public Type ValueType { get; }
    }
}