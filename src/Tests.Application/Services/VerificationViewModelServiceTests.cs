﻿using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Devices;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Moq;
using Newtonsoft.Json;
using Prover.Application.Services;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;
using Tests.Application;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Application.Services.Tests
{
    [TestClass()]
    public class VerificationViewModelServiceTests
    {
        private Mock<DeviceInstance> _instance;
        private DeviceInstance _device;
        private DeviceType _deviceType;
        private IDeviceRepository _repo;

        private Mock<IAsyncRepository<EvcVerificationTest>> _repoMock = new Mock<IAsyncRepository<EvcVerificationTest>>();
        private Mock<EvcVerificationTestService> _serviceMock = new Mock<EvcVerificationTestService>();

        private EvcVerificationTestService _testService;
        private VerificationViewModelService _service;
        //private IAsyncRepository<EvcVerificationTest> _repository;

        //private static LiteDatabase 

        [TestInitialize]
        public async Task Init()
        {
            _serviceMock = new Mock<EvcVerificationTestService>(_repoMock.Object);
            
            _service = new VerificationViewModelService(_serviceMock.Object);

            _instance = new Mock<DeviceInstance>();
            _repo = await RepositoryFactory.CreateDefaultAsync();
            _deviceType = _repo.GetByName("Mini-Max");

            var items = ItemFiles.MiniMaxItemFile;
            _device = _deviceType.CreateInstance(items);
        }

        [TestMethod()]
        public void VerificationViewModelServiceTest()
        {
        }

        [TestMethod()]
        public async Task CreateVerificationTestFromViewModelTest()
        {
            var newTest = _service.NewTest(_device);

            var model = _service.CreateVerificationTestFromViewModel(newTest);
            Assert.AreEqual(newTest.Id, model.Id);
            
            var viewModel = await _service.GetVerificationTest(model);
            newTest.WithDeepEqual(viewModel)
                .IgnoreSourceProperty(t => t.Device.DeviceType)
                .Assert();

            var model2 = _service.CreateVerificationTestFromViewModel(viewModel);
            model2.ShouldDeepEqual(model);

            //Assert.IsTrue(model2.IsDeepEqual(model));
            //Assert.IsTrue(newTest.IsDeepEqual(viewModel));
        }

        [TestMethod()]
        public void GetVerificationTestsTest()
        {
        }

        [TestMethod()]
        public void NewTestTest()
        {
            var newTest = _service.NewTest(_device);

            Assert.IsNotNull(newTest);
        }
    }
}