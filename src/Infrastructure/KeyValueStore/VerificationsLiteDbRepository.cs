using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Domain.EvcVerifications;
using LiteDB;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infrastructure.KeyValueStore
{
    public class VerificationsLiteDbRepository : LiteDbRepository<EvcVerificationTest>
    {
        private readonly DeviceRepository _deviceRepository;

        public VerificationsLiteDbRepository(ILiteDatabase context, DeviceRepository deviceRepository) : base(context)
        {
            _deviceRepository = deviceRepository;
            Initialize();
        }
        private void Initialize()
        {
            var mapper = BsonMapper.Global;

            mapper.RegisterType<DeviceType>
            (
                (d) => d.Id,
                (bson) => _deviceRepository.GetById(bson)
            );

            mapper.RegisterType<DeviceInstance>
            (
                (d) =>  JsonSerializer.Serialize(new Device(d)),
                (bson) =>
                {
                    var temp = JsonSerializer.Deserialize<Device>(bson);
                    var device = _deviceRepository.GetById(temp.DeviceTypeId);
                    return device.Factory.CreateInstance(temp.Values);
                });
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