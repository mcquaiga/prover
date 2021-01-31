using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using LiteDB;
using Newtonsoft.Json;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;

namespace Prover.Storage.LiteDb
{
	public static class LiteDbStorageDefaults
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

		public static IRepository<DeviceType> CreateDefaultDeviceTypeRepository() => new DeviceTypeLiteDbRepository(Database);

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
				var col = db.GetCollection<EvcVerificationTest>();
				col.EnsureIndex(test => test.TestDateTime);
				col.EnsureIndex(test => test.ExportedDateTime);
				col.EnsureIndex(test => test.Archived);
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

	//public class LiteDbInitializer
	//{
	//    private readonly IAsyncRepository<EvcVerificationTest> _testRepository;
	//    private readonly DeviceRepository _deviceRepository;
	//    private readonly EvcVerificationTestService _service;
	//    private readonly VerificationViewModelService _viewModelService;

	//    public LiteDbInitializer(IAsyncRepository<EvcVerificationTest> testRepository, DeviceRepository deviceRepository, EvcVerificationTestService service, VerificationViewModelService viewModelService)
	//    {
	//        _testRepository = testRepository;
	//        _deviceRepository = deviceRepository;
	//        _service = service;
	//        _viewModelService = viewModelService;
	//    }

	//    public LiteDbInitializer(DeviceRepository deviceRepository)
	//    {
	//        _deviceRepository = deviceRepository;
	//    }

	//    public void Initialize()
	//    {
	//        var mapper = BsonMapper.Global;

	//        mapper.RegisterType<DeviceType>
	//        (
	//            serialize: (d) => d.Id,
	//            deserialize: (bson) => _deviceRepository.GetById(bson)
	//        );

	//        mapper.RegisterType<DeviceInstance>
	//        (
	//            serialize: (d) => JsonSerializer.Serialize(new Device(d)),
	//            deserialize: (bson) =>
	//            {
	//                var temp = JsonSerializer.Deserialize<Device>(bson);
	//                var device = _deviceRepository.GetById(temp.DeviceTypeId);
	//                return device.Factory.CreateInstance(temp.Values);
	//            });

	//        //mapper.Entity<VerificationTestPoint>()
	//        //    .Ctor(doc => new VerificationTestPoint(doc["Tests"].AsArray));
	//    }

	//    private async Task Seed()
	//    {
	//        var repo = await Devices.RepositoryFactory.CreateDefaultAsync();

	//        var deviceType = repo.GetByName("Mini-Max");
	//        var device = deviceType.Factory.CreateInstance(ItemFiles.MiniMaxItemFile);

	//        var lowTemp = deviceType.ToItemValuesEnumerable(ItemFiles.TempLowItems);
	//        var ti = device.ItemGroup<TemperatureItems>(lowTemp);

	//        //CorrectionTest.Update<TemperatureItems>(tempTest, ti, ti.Factor)
	//        var tempVm = new TemperatureFactorViewModel(ti, 32);

	//        var testVm = _viewModelService.NewTest(device);
	//        var t = _viewModelService.CreateVerificationTestFromViewModel(testVm);

	//        await _testRepository.AddAsync(t);

	//        var testsList = await _testRepository.ListAsync();

	//        //var myVm = await _viewModelService.CreateViewModelFromVerification(testsList.First());
	//    }

	//    private async Task Load()
	//    {
	//        var my = await _testRepository.ListAsync();

	//    }

	//    private class Device
	//    {
	//        private Device()
	//        {

	//        }

	//        public Device(DeviceInstance value)
	//        {
	//            DeviceTypeId = value.DeviceType.Id;
	//            Values = value.Values.ToItemValuesDictionary();
	//        }

	//        public Guid DeviceTypeId { get; set; }
	//        public Dictionary<string, string> Values { get; set; }
	//    }
	//}
}