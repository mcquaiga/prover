using DeepEqual.Syntax;
using Devices;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Domain.EvcVerifications;
using DynamicData;
using Infrastructure.KeyValueStore;
using LiteDB;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI;
using Shared.Interfaces;
using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private IAsyncRepository<EvcVerificationTest> _testRepo;
        private VerificationViewModelService _viewModelService;
        private EvcVerificationTestService _modelService;

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
                //newTest.ExportedDateTime = DateTime.Now;
                var model = _viewModelService.CreateVerificationTestFromViewModel(newTest);
                await _testRepo.AddAsync(model);
                x++;
            } while (x < 10);

            var tests = await _testRepo.ListAsync();
            var views = await _viewModelService.GetVerificationTests(tests);

            Assert.IsFalse(views.ElementAt(0).IsDeepEqual(views.ElementAt(5)));
        }

        [TestMethod]
        public async Task CreateObservableStream()
        {
            var scheduler = new TestScheduler();
            scheduler.StartStopwatch();
            
            var results = _modelService.VerificationTests.Connect(t => t.ExportedDateTime == null)
                //.Transform(t =>  _viewModelService.GetTest(t))
                //.ObserveOn(Scheduler.Default)
                .Bind(out var data, 25)
                //.DisposeMany()
                .Subscribe();
           
            //scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);
            scheduler.Start();
            Assert.IsNotNull(data);
            //var evc = new SourceList<EvcVerificationTest>();
            //_testRepo.List()
            //    .Subscribe(t => evc.Add(t));

            //evc.Connect()
            //    .TransformAsync(t => _viewModelService.GetTest(t))
            //    .Bind(out var data)
            //    .Subscribe();
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
            _modelService = new EvcVerificationTestService(_testRepo);
            _viewModelService = new VerificationViewModelService(_modelService);
        }
    }

    [TestClass]
    public class EvcVerificationTestServiceReactiveTests : ReactiveTest
    {

    }
}