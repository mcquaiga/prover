//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Application.Services;
//using Application.ViewModels.Corrections;
//using Devices.Core.Interfaces;
//using Devices.Core.Items;
//using Devices.Core.Items.ItemGroups;
//using Devices.Core.Repository;
//using Domain.EvcVerifications;
//using Infrastructure.SampleData;
//using LiteDB;
//using Shared.Interfaces;
//using JsonSerializer = System.Text.Json.JsonSerializer;

//namespace Infrastructure.KeyValueStore
//{
//    public class LiteDbInitializer
//    {
//        private readonly IAsyncRepository<EvcVerificationTest> _testRepository;
//        private readonly DeviceRepository _deviceRepository;
//        private readonly EvcVerificationTestService _service;
//        private readonly VerificationViewModelService _viewModelService;

//        public LiteDbInitializer(IAsyncRepository<EvcVerificationTest> testRepository, DeviceRepository deviceRepository, EvcVerificationTestService service, VerificationViewModelService viewModelService)
//        {
//            _testRepository = testRepository;
//            _deviceRepository = deviceRepository;
//            _service = service;
//            _viewModelService = viewModelService;
//        }

//        public LiteDbInitializer(DeviceRepository deviceRepository)
//        {
//            _deviceRepository = deviceRepository;
//        }

//        public void Initialize()
//        {
//            var mapper = BsonMapper.Global;

//            mapper.RegisterType<DeviceType>
//            (
//                serialize: (d) => d.Id,
//                deserialize: (bson) => _deviceRepository.GetById(bson)
//            );

//            mapper.RegisterType<DeviceInstance>
//            (
//                serialize: (d) =>  JsonSerializer.Serialize(new Device(d)),
//                deserialize: (bson) =>
//                {
//                    var temp = JsonSerializer.Deserialize<Device>(bson);
//                    var device = _deviceRepository.GetById(temp.DeviceTypeId);
//                    return device.Factory.CreateInstance(temp.Values);
//                });

//            //mapper.Entity<VerificationTestPoint>()
//            //    .Ctor(doc => new VerificationTestPoint(doc["Tests"].AsArray));
//        }

//        private async Task Seed()
//        {
//            var repo = await Devices.RepositoryFactory.CreateDefaultAsync();

//            var deviceType = repo.GetByName("Mini-Max");
//            var device = deviceType.Factory.CreateInstance(ItemFiles.MiniMaxItemFile);

//            var lowTemp = deviceType.ToItemValuesEnumerable(ItemFiles.TempLowItems);
//            var ti = device.ItemGroup<TemperatureItems>(lowTemp);

//            //CorrectionTest.Update<TemperatureItems>(tempTest, ti, ti.Factor)
//            var tempVm = new TemperatureFactorViewModel(ti, 32);

//            var testVm = _viewModelService.NewTest(device);
//            var t = _viewModelService.CreateVerificationTestFromViewModel(testVm);

//            await _testRepository.AddAsync(t);

//            var testsList = await _testRepository.ListAsync();

//            //var myVm = await _viewModelService.CreateViewModelFromVerification(testsList.First());
//        }

//        private async Task Load()
//        {
//            var my = await _testRepository.ListAsync();

//        }

//        private class Device
//        {
//            private Device()
//            {

//            }

//            public Device(DeviceInstance value)
//            {
//                DeviceTypeId = value.DeviceType.Id;
//                Values = value.Values.ToItemValuesDictionary();
//            }

//            public Guid DeviceTypeId { get; set; }
//            public Dictionary<string, string> Values { get; set; }
//        }
//    }
//}