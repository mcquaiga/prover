using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Security.Cryptography;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using LiteDB;
using Newtonsoft.Json;
using Prover.Application.Interactions;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using Prover.Infrastructure.KeyValueStore;
using Prover.Shared.Interfaces;

// ReSharper disable RedundantTypeArgumentsOfMethod

namespace Prover.Infrastructure
{
    public static class StorageDefaults
    {
        private static readonly Lazy<ILiteDatabase> Lazy =
            new Lazy<ILiteDatabase>(CreateDatabaseForLazy);

        public static string ConnectionString { get; private set; } = "prover_data.db";
        public static ILiteDatabase Database => Lazy.Value;

        public static IDeviceTypeCacheSource<DeviceType> CreateDefaultDeviceTypeCache()
        {
            var repo = CreateDefaultDeviceTypeRepository();
            return new DeviceTypeCacheSource(repo);
        }

        public static IRepository<DeviceType> CreateDefaultDeviceTypeRepository() =>
            new DeviceTypeLiteDbRepository(Database);

        public static ILiteDatabase CreateLiteDb(string path = null)
        {
            if (!Lazy.IsValueCreated && !string.IsNullOrEmpty(path))
                ConnectionString = path;

            return Database;
        }

        private static void ConfigureMappings(ILiteDatabase db = null)
        {
            db = db ?? Database;
            var jsonSettings = new JsonSerializerSettings();

            var mapper = BsonMapper.Global;

            mapper.RegisterType<DeviceInstance>(
                d =>
                {
                    var device = new Device(d);
                    return JsonConvert.SerializeObject(device);
                },
                bson =>
                {
                    var temp = JsonConvert.DeserializeObject<Device>(bson);
                    var device = DeviceRepository.Instance.GetById(temp.DeviceTypeId);
                    return device.CreateInstance(temp.Values);
                });

            if (db.CollectionExists("EvcVerificationTest"))
            {
                //db.GetCollection<EvcVerificationTest>()..Exists(t => t.ExportedDateTime)
                db.GetCollection<EvcVerificationTest>().EnsureIndex(test => test.TestDateTime);
                //db.GetCollection<EvcVerificationTest>().EnsureIndex(test => test.ArchivedDateTime);
            }

            mapper.Entity<DeviceType>().Ignore(d => d.Factory);
        }

        private static ILiteDatabase CreateDatabaseForLazy()
        {
            try
            {
                var db = new LiteDatabase(ConnectionString);
                ConfigureMappings(db);
                return db;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        
        
        #region Nested type: Device

        private class Device
        {
            public Device()
            {
            }

            [JsonConstructor]
            public Device(Guid deviceTypeId, Dictionary<string, string> values)
            {
                DeviceTypeId = deviceTypeId;
                Values = values;
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