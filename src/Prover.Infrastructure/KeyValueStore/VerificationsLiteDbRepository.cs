using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using LiteDB;
using Prover.Domain.EvcVerifications;
using System;
using System.Collections.Generic;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Prover.Infrastructure.KeyValueStore
{
    public class VerificationsLiteDbRepository : LiteDbAsyncRepository<EvcVerificationTest>
    {
        private readonly DeviceRepository _deviceRepository;

        public VerificationsLiteDbRepository(ILiteDatabase context, DeviceRepository deviceRepository) : base(context)
        {
            
            _deviceRepository = deviceRepository;
            Initialize();
        }

        internal void Initialize()
        {
            var mapper = BsonMapper.Global;
            
            mapper.RegisterType<DeviceType>
            (
                d => d.Id,
                bson => _deviceRepository.GetById(bson)
            );

            mapper.RegisterType<DeviceInstance>
            (
                d => JsonSerializer.Serialize(new Device(d)),
                bson =>
                {
                    var temp = JsonSerializer.Deserialize<Device>(bson);
                    var device = _deviceRepository.GetById(temp.DeviceTypeId);
                    return device.CreateInstance(temp.Values);
                });

            mapper.Entity<EvcVerificationTest>()
                .Ignore(v => v.DeviceType);

            Context.GetCollection<EvcVerificationTest>().EnsureIndex(test => test.ExportedDateTime);
            Context.GetCollection<EvcVerificationTest>().EnsureIndex(test => test.ArchivedDateTime);
            //Context.GetCollection<EvcVerificationTest>().EnsureIndex(test => test.DeviceType);

        }

        #region Nested type: Device

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

            public Guid DeviceTypeId { get; }
            public Dictionary<string, string> Values { get; }
        }

        #endregion
    }
}