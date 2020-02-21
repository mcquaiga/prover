using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Application.ViewModels;
using Application.ViewModels.Corrections;
using DeepEqual.Syntax;
using Devices;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Items;
using Domain.EvcVerifications;
using Infrastructure.EntityFrameworkSqlDataAccess.Repositories;
using Infrastructure.KeyValueStore;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared.Interfaces;
using Tests.Shared;

namespace Application.Services.Tests
{
    [TestClass]
    public class EvcVerificationTestServiceTests
    {
        private static readonly LiteDatabase _db = new LiteDatabase("test_data.db");
        private DeviceInstance _device;
        private DeviceType _deviceType;
        private DeviceRepository _repo;

        private Mock<IAsyncRepository<EvcVerificationTest>> _repoMock =
            new Mock<IAsyncRepository<EvcVerificationTest>>();

        private IAsyncRepository<EvcVerificationTest> _testRepo;
        private VerificationViewModelService _viewModelService;

        [TestMethod]
        public async Task AddOrUpdateVerificationTestTest()
        {
            var newTest = _viewModelService.NewTest(_device);

            var model = _viewModelService.CreateVerificationTestFromViewModel(newTest);

            await _testRepo.AddAsync(model);

            var model2 = await _testRepo.GetAsync(model.Id);
            Assert.IsNotNull(model2);

            model.WithDeepEqual(model2)
                .IgnoreSourceProperty(p => p.TestDateTime)
                .Assert();

            Assert.AreEqual(model.TestDateTime.ToString(CultureInfo.InvariantCulture),
                model2.TestDateTime.ToString(CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task EvcVerificationTestServiceTest()
        {
            var tests = await _testRepo.ListAsync();

            var success = tests.All(t => t.Tests.OfType<VerificationTestPoint>().Count() == 3);
            success = tests.All(t => t.Tests.OfType<VerificationTestPoint>().All(p => p.Tests.Count > 0));
        }

        [TestMethod]
        public async Task CreateAndSaveToDatabaseTest()
        {
            var x = 0;
            do
            {
                var newTest = _viewModelService.NewTest(_device);
                var model = _viewModelService.CreateVerificationTestFromViewModel(newTest);
                await _testRepo.AddAsync(model);
                x++;
            } while (x < 10);

            var tests = await _testRepo.ListAsync();
            var views = await _viewModelService.GetVerificationTests(tests.Take(10).ToList());
            
            //views.ElementAt(0).ShouldDeepEqual(views.ElementAt(5));
            
            Assert.IsFalse(views.ElementAt(0).IsDeepEqual(views.ElementAt(5)));
        }

        [TestMethod]
        public async Task CreateTestAndLoadFromLiteDbTest()
        {
            var newTest = _viewModelService.NewTest(_device);
            var model = _viewModelService.CreateVerificationTestFromViewModel(newTest);
            await _testRepo.AddAsync(model);

            var dbObject = await _testRepo.GetAsync(model.Id);

            model.WithDeepEqual(dbObject)
                .IgnoreSourceProperty(s => s.TestDateTime)
                .Assert();

            var success = dbObject.Tests.OfType<VerificationTestPoint>().Count() == 3;
            success = success && dbObject.Tests.OfType<VerificationTestPoint>().All(p => p.Tests.Count >= 3);
            Assert.IsTrue(success);
        }

        [TestInitialize]
        public async Task Init()
        {
            _repo = await RepositoryFactory.CreateDefaultAsync();
            _deviceType = _repo.GetByName("Mini-Max");
            _device = _deviceType.CreateInstance(ItemFiles.MiniMaxItemFile);


            _testRepo = new VerificationsLiteDbRepository(_db, _repo);
            _viewModelService = new VerificationViewModelService(_testRepo);
        }
    }
}