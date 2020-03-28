using DeepEqual.Syntax;
using Devices;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using DynamicData;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Microsoft.Extensions.Logging;
using Prover.Application.Services;
using Prover.Domain.EvcVerifications;
using Prover.Infrastructure;
using Prover.Infrastructure.KeyValueStore;
using Prover.Shared.Interfaces;
using Tests.Application;
using Devices.Romet.Core.Repository;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume.Factories;

namespace Application.Services.Tests
{
    [TestClass]
    public class EvcVerificationTestServiceTests
    {
        private DeviceInstance _device;
        private DeviceType _deviceType;
        private IDeviceRepository _repo;

        private IAsyncRepository<EvcVerificationTest> _testRepo;
        private VerificationTestService<EvcVerificationTest, EvcVerificationViewModel> _viewModelService;
        private EvcVerificationTestService<EvcVerificationTest> _modelService;

        [TestMethod]
        public async Task AddOrUpdateVerificationTestTest()
        {
            var newTest = _viewModelService.NewTest(_device);

            var model = _viewModelService.CreateVerificationTestFromViewModel(newTest);

            await _testRepo.AddAsync(model);

            var model2 = await _testRepo.GetAsync(model.Id);
            Assert.IsNotNull(model2);

            model2.WithDeepEqual(model)
                .IgnoreSourceProperty(p => p.TestDateTime)
                .Assert();

            Assert.AreEqual(model.TestDateTime.ToString(CultureInfo.InvariantCulture),
                model2.TestDateTime.ToString(CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task EvcVerificationTestServiceTest()
        {
            await CreateAndSaveNewTest();
            var tests = await _testRepo.ListAsync();
            var success = tests.All(t => t.Tests.OfType<VerificationTestPoint>().Count() == 3);
            Assert.IsTrue(success);
            success = tests.All(t => t.Tests.OfType<VerificationTestPoint>().All(p => p.Tests.Count > 0));
            Assert.IsTrue(success);
            success = tests.All(t => t.Tests.OfType<VerificationTestPoint>().All(p => p.Tests.All(v => v != null)));
            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task CreateAndSaveToDatabaseTest()
        {
            var x = 0;
            do
            {
                var newTest = await CreateAndSaveNewTest();
                var dbTest = await _testRepo.GetAsync(newTest.Id);
                Assert.IsNotNull(dbTest);
                x++;
            } while (x < 5);
            //var views = await _viewModelService.GetVerificationTests(tests);

            //Assert.IsFalse(views.ElementAt(0).IsDeepEqual(views.ElementAt(5)));
        }

        private async Task<EvcVerificationViewModel> CreateAndSaveNewTest()
        {
            var newTest = _viewModelService.NewTest(_device);
            //newTest.ExportedDateTime = DateTime.Now;
            var model = _viewModelService.CreateVerificationTestFromViewModel(newTest);
            await _testRepo.AddAsync(model);
            return newTest;
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
            _repo = new DeviceRepository();
            await _repo.UpdateCachedTypes(MiJsonDeviceTypeDataSource.Instance);
            await _repo.UpdateCachedTypes(RometJsonDeviceTypeDataSource.Instance);
            _deviceType = _repo.GetByName("Mini-Max");
            _device = _deviceType.CreateInstance(ItemFiles.MiniMaxItemFile);

            _testRepo = new VerificationsLiteDbRepository(StorageDefaults.Database, _repo);
            _modelService = new EvcVerificationTestService<EvcVerificationTest>(_testRepo);
            _viewModelService = new VerificationTestService<EvcVerificationTest, EvcVerificationViewModel>(_testRepo, new VerificationViewModelFactory(), null, null);
        }
    }

    [TestClass]
    public class EvcVerificationTestServiceReactiveTests : ReactiveTest
    {

    }

    //class RepositoryFactory
    //{
    //    //public RepositoryFactory(ILogger logger)
    //    //{
    //    //}

    //    //if (DeviceRepository.Instance == null)
    //    //    throw new InvalidOperationException("Call Create method first.");
    //    public static IDeviceRepository Instance => DeviceRepository.Instance;

    //    public static IDeviceRepository Get => Instance;

    //    public static async Task<IDeviceRepository> Create(IDeviceTypeCacheSource<DeviceType> cacheSource,
    //        IEnumerable<IDeviceTypeDataSource<DeviceType>> sources = null)
    //    {
    //        var instance = Instance;

    //        if (Instance == null)
    //        {
    //            instance = new DeviceRepository(cacheSource);
    //        }

    //        await instance.UpdateCachedTypes();

    //        if (instance.Devices.Count == 0 && sources != null)
    //            await instance.UpdateCachedTypes(sources);

    //        return instance;
    //    }

    //    public static async Task<IDeviceRepository> Create(IEnumerable<IDeviceTypeDataSource<DeviceType>> sources)
    //    {
    //        var instance = Instance;

    //        if (Instance == null)
    //            instance = new DeviceRepository(null);

    //        await instance.UpdateCachedTypes(sources);

    //        return instance;
    //    }

    //    public static IDeviceRepository CreateDefault()
    //    {
    //        var task = Task.Run(CreateDefaultAsync);
    //        task.Wait();
    //        return task.Result;
    //    }

    //    public static async Task<IDeviceRepository> CreateDefaultAsync()
    //    {
    //        return await Create(StorageDefaults.CreateDefaultDeviceTypeCache(),
    //            new[] { MiJsonDeviceTypeDataSource.Instance, RometJsonDeviceTypeDataSource.Instance });
    //    }
    //}

}