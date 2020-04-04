using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using DynamicData;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;
using Tests.Shared;

namespace Tests.Application.Services
{
    [TestClass]
    public class EvcVerificationTestServiceTests
    {
        private static DeviceInstance _device;
        private static DeviceType _deviceType;
        private static IDeviceRepository _repo;

        private static IAsyncRepository<EvcVerificationTest> _testRepo;
        private static IVerificationTestService _viewModelService;
        private static EvcVerificationTestService _modelService;

        [TestMethod]
        public async Task AddOrUpdateVerificationTestTest()
        {
            var newTest = _viewModelService.NewTest(_device);
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
           
            await _viewModelService.AddOrUpdate(newTest);
           
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
            var newTest = await CreateAndSaveNewTest();
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
            _device = _deviceType.CreateInstance(ItemFiles.MiniMaxItemFile);
        }

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            _repo = StorageInitialize.Repo;
            _deviceType = _repo.GetByName("Mini-Max");

            _testRepo = StorageInitialize.TestRepo;
            _modelService = StorageInitialize.ModelService;
            _viewModelService =StorageInitialize.ViewModelService;
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