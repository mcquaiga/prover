using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Application.FileLoader;
using Prover.Application.Interfaces;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.Verifications;
using Prover.Application.ViewModels.Corrections;
using Prover.Domain.EvcVerifications;
using Prover.Domain.EvcVerifications.Verifications.CorrectionFactors;
using Tests.Application.Services;

namespace Tests.Application.FileLoader
{
    [TestClass()]
    public class ItemFilesTests
    {
        private string _filePath = ".\\MiniMax.json";
        private string _templatefilePath = ".\\Template.json";
        private ItemAndTestFile _itemFile;
        private Mock<IDeviceSessionManager> _deviceManagerMock = new Mock<IDeviceSessionManager>();
        private Mock<LiveReadCoordinator> _liveReadMock = new Mock<LiveReadCoordinator>();

        [TestInitialize]
        public async Task Init()
        {
            _itemFile = await ItemLoader.LoadFromFile(StorageTestsInitialize.DeviceRepo, _filePath);
        }

        [TestMethod()]
        public async Task LoadFromFileTest()
        {
            var items = await ItemLoader.LoadFromFile(StorageTestsInitialize.DeviceRepo, _filePath);
            
            Assert.IsTrue(items != null);
        }  

        [TestMethod()]
        public async Task LoadFromTemplate()
        {
            var loader = new ItemLoader(StorageTestsInitialize.DeviceRepo, StorageTestsInitialize.ViewModelService);
            
            var test = await loader.LoadTemplate(_templatefilePath);
            
            Assert.IsTrue(test != null);
        }  
        
        [TestMethod()]
        public async Task FileCommunicationClientInstanceTest()
        {
            var client = new FileDeviceClient(_itemFile);
           // var manager = new Device
            //VerificationEvents.CorrectionTests.OnLiveReadStart.Publish(new LiveReadCoordinator(_deviceManagerMock.Ob));
            var tests = await StorageTestsInitialize.TestRepo.ListAsync();

            //var test = tests.Last().VerificationTestMixins.GetTests<VerificationTestPoint>().Select(p => p.GetTests<TemperatureCorrectionTest>()).ToList();
            var test = tests.First();

            var deviceRepo = StorageTestsInitialize.DeviceRepo;
            //var serializeOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true};
            //serializeOptions.Converters.Add(new DeviceInstanceJsonConverter(StorageTestsInitialize.DeviceRepo));

            var json = JsonSerializer.Serialize(test.Device);

            var instance = JsonSerializer.Deserialize<DeviceInstance>(json);


            Assert.IsTrue(instance != null);
            
            Assert.IsTrue(json != null);
        }

    }
}

//var json = JsonConvert.SerializeObject(test).Tests.Tests.GetCorrectionTests<PressureItems>();


////.GetTests<VerificationTestPoint>().Select(p => p.GetTests<TemperatureCorrectionTest>()).ToList();

//tests.Take(5).ForEach(t => new ItemAndTestFile()
//{
//    Device = t.Device,
//    TemperatureTests = t.S.Tests.OfType<VerificationTestPoint>()
//                        .SelectMany(i => new KeyValuePair<int, List<ItemValue>>(i.TestNumber, i.Tests.GetCorrectionTests<TemperatureItems>().ToDictionary(k => k.)

//                                                              .SelectMany(p => p.Tests.OfType<TemperatureCorrectionTest>().ToList().SelectMany(x => x.)
//                                                                                .S

////})
//var items = new ItemAndTestFile()
//{
//    VolumeTest = Tuple.Create(new)
//}