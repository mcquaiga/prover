using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;
using ReactiveUI;
using Tests.Application.ExternalDevices.DInOutBoards;
using Tests.Shared;

namespace Tests.Application.Services
{
    [TestClass]
    public class VerificationTestServiceLiteDbTests : ReactiveTest
    {
        private static TestSchedulers _testSchedulers;
        private CompositeDisposable _cleanup;
        private static DeviceInstance _device;
        private static DeviceType _deviceType;
        private static IDeviceRepository _repo;

        private static IAsyncRepository<EvcVerificationTest> _testRepo;
        private static IVerificationTestService _viewModelService;
        private static IEntityDataCache<EvcVerificationTest> _entityCache;
      

        [TestMethod]
        public async Task AddOrUpdateVerificationTestTest()
        {
            var newTest = _viewModelService.NewVerification(_device);
            await _viewModelService.AddOrUpdate(newTest);

            var model = _viewModelService.CreateModel(newTest);
            await _viewModelService.AddOrUpdate(model);

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
            await StorageTestsInitialize.CreateAndSaveNewTest(_device);
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
            var newTest = await StorageTestsInitialize.CreateAndSaveNewTest(_device);
          
            var dbTest = await _testRepo.GetAsync(newTest.Id);
            Assert.IsNotNull(dbTest);
        }

        [TestMethod]
        public async Task CreateObservableStream()
        {
            var scheduler = new TestScheduler();
            scheduler.StartStopwatch();
            
            var results = _entityCache.Updates.Connect(t => t.ExportedDateTime == null)
                                      .Bind(out var data, 25)
                                      .Subscribe();
            
            scheduler.Start();
            Assert.IsNotNull(data);
            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task LoadVerificationsObservableAndMonitorForChanges()
        {
            var scheduler = new TestScheduler();
            var id = Guid.NewGuid();
            var initNumber = 2;
            var testService = StorageTestsInitialize.CreateVerificationTestService();
            var updatesCount = 0;

            StorageTestsInitialize.DropCollection();

            testService.Load().Connect()
                .Bind(out var tests)
                .Subscribe(x => updatesCount++);

            scheduler.Start();

            var myTests = await StorageTestsInitialize.CreateAndSaveNewTests(_deviceType, ItemFiles.MiniMaxItemFile, initNumber, testService);
            Assert.IsTrue(tests.Count >= initNumber);

            var newTest = await StorageTestsInitialize.CreateAndSaveNewTest(_deviceType, ItemFiles.MiniMaxItemFile, testService);
            Assert.IsTrue(tests.Count >= initNumber + 1);

            newTest.JobId = "123456";
            await testService.AddOrUpdate(newTest);
            Assert.IsTrue(tests.Any(t => t.JobId == newTest.JobId));
            Assert.IsTrue(tests.Count >= initNumber + 1);
            
            Assert.IsTrue(updatesCount >= 4);
        }

        [TestMethod]
        public async Task ConcurrentlyAccessDeviceType()
        {
            var tasks = new List<Task<EvcVerificationViewModel>>();

            for (int i = 0; i < 5; i++)
            {
                
                tasks.Add(Task.Run(async () =>
                {
                    var device = _deviceType.CreateInstance(ItemFiles.MiniMaxItemFile);
                    return await StorageTestsInitialize.CreateAndSaveNewTest(device);
                }));
            }

            var myTests = await Task.WhenAll(tasks);
            Assert.IsTrue(myTests.Any());

        }

        [TestMethod]
        public async Task CreateTestAndLoadFromLiteDbTest()
        {
            var newTest = await StorageTestsInitialize.CreateAndSaveNewTest(_device);
            var model = _viewModelService.CreateModel(newTest);
            
            var dbObject = await _testRepo.GetAsync(model.Id);

            var tests = dbObject.Tests.OfType<VerificationTestPoint>().ToList();
            var json = JsonConvert.SerializeObject(tests,
                new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            Debug.WriteLine(json);

            model.WithDeepEqual(dbObject)
                .IgnoreSourceProperty(s => s.TestDateTime)
                .Assert();

            Assert.IsTrue(tests.Count() == newTest.VerificationTests.OfType<VerificationTestPointViewModel>().Count());

            var childCount = tests.SelectMany(t => t.Tests).Count();
            var modelChildCount = 
                model.Tests.OfType<VerificationTestPoint>().SelectMany(t => t.Tests).Count();
            
            Assert.IsTrue(childCount == modelChildCount);
 
        }

        [TestInitialize]
        public async Task Init()
        {
            _cleanup = new CompositeDisposable();
            _device = _deviceType.CreateInstance(ItemFiles.MiniMaxItemFile);

            await Task.CompletedTask;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _cleanup.Dispose();
        }

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            _repo = StorageTestsInitialize.DeviceRepo;
            _deviceType = _repo.GetByName("Mini-Max");

            _testRepo = StorageTestsInitialize.TestRepo; 
            _viewModelService =StorageTestsInitialize.ViewModelService;
            await Task.CompletedTask;
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {

            await Task.CompletedTask;
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