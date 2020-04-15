﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Prover.Application.Extensions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Factories;
using Prover.Domain.EvcVerifications;
using Prover.Infrastructure.KeyValueStore;
using Prover.Infrastructure.SampleData;
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

    public class DatabaseSeeder
    {
        public Task SeedDatabase(IRepository<EvcVerificationTest> repository, int records = 1)
        {  
            Debug.WriteLine($"Seeding data...");
            var watch = Stopwatch.StartNew();
            
            var results = CreateTests(records);
       
            _ = Task.Run(() => results.ForEach(t => repository.Update(t)));

            watch.Stop();
            Debug.WriteLine($"Seeding completed in {watch.ElapsedMilliseconds} ms");
            return Task.CompletedTask;
        }

        public IEnumerable<EvcVerificationTest> CreateTests(int records = 1)
        {
           
            var results = new List<EvcVerificationTest>();
            var random = new Random(DateTime.Now.Millisecond);
            

            var deviceType = DeviceRepository.Instance.GetByName("Mini-Max");
          
            
            var serialNumberItem = deviceType.GetItemMetadata(62);

            for (int i = 0; i < records; i++)
            {
                var device = deviceType.CreateInstance(ItemFiles.MiniMaxItemFile);
                device.SetItemValue(serialNumberItem, random.Next(10000, 999999).ToString());
                device.SetItemValue(201, random.Next(10000, 999999).ToString());

                var testVm = VerificationViewModelFactory.Create(device);

                testVm.SubmittedDateTime = testVm.TestDateTime.AddSeconds(random.Next(180, 720));

                testVm.SetItems<TemperatureItems>(device, 0, ItemFiles.TempLowItems);
                testVm.SetItems<TemperatureItems>(device, 1, ItemFiles.TempMidItems);
                testVm.SetItems<TemperatureItems>(device, 2, ItemFiles.TempHighItems);
                
                testVm.SetItems<PressureItems>(device, 0, ItemFiles.PressureTest(0));
                testVm.SetItems<PressureItems>(device, 1, ItemFiles.PressureTest(1));
                testVm.SetItems<PressureItems>(device, 2, ItemFiles.PressureTest(2));
                
                results.Add(testVm.ToModel());
                

                //await testService.AddOrUpdate(testVm);

                Debug.WriteLine($"Created verification test {i} of {records}.");

            }

            return results;
        }

    }
}