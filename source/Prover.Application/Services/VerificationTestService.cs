using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume.Factories;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;

namespace Prover.Application.Services
{
    public class VerificationTestService : IVerificationTestService, IDisposable
    {
        private readonly IDeviceSessionManager _deviceManager;

        private readonly Func<DeviceInstance, EvcVerificationTest> _evcVerificationTestFactory;

        private readonly ISubject<EvcVerificationTest> _updates = new Subject<EvcVerificationTest>();

        private ISourceCache<EvcVerificationTest, Guid> _testsCache
            = new SourceCache<EvcVerificationTest, Guid>(k => k.Id);

        private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;
        private readonly ITestManagerFactory _verificationManagerFactory;
        private readonly IVerificationViewModelFactory _verificationViewModelFactory;
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();

        private IConnectableObservable<IChangeSet<EvcVerificationTest, Guid>> _tests;

        private IObservableCache<EvcVerificationTest, Guid> _verificationTests;
        private readonly object _lock = new AsyncLock();

        public VerificationTestService(
            IAsyncRepository<EvcVerificationTest> verificationRepository,
            IVerificationViewModelFactory verificationViewModelFactory,
            IDeviceSessionManager deviceManager,
            ITestManagerFactory verificationManagerFactory,
            Func<DeviceInstance, EvcVerificationTest> evcVerificationTestFactory = null,
            IScheduler scheduler = null)
        {
            _verificationRepository = verificationRepository;
            _verificationViewModelFactory = verificationViewModelFactory;
            _deviceManager = deviceManager;
            _verificationManagerFactory = verificationManagerFactory;
            _evcVerificationTestFactory = evcVerificationTestFactory;

            //var updateConnect = _updates.Publish();
            //Updates = updateConnect;
            //updateConnect.Connect().DisposeWith(_cleanup);

            //All = new SourceCache<EvcVerificationTest, Guid>(v => v.Id);
        }
        //public IObservableCache<EvcVerificationTest, Guid> All { get; }
        //public IObservable<EvcVerificationTest> Updates { get; }

        public async Task<EvcVerificationViewModel> AddOrUpdate(EvcVerificationViewModel viewModel)
        {
            viewModel.TestDateTime = viewModel.TestDateTime ?? DateTime.Now;

            var model = 
                await AddOrUpdate(
                    VerificationMapper.MapViewModelToModel(viewModel));

            return model.ToViewModel();
        }

        public async Task<EvcVerificationTest> AddOrUpdate(EvcVerificationTest evcVerificationTest)
        {
            await _verificationRepository.UpsertAsync(evcVerificationTest);
          
            _testsCache.AddOrUpdate(evcVerificationTest);

            return evcVerificationTest;
        }

        public EvcVerificationTest CreateModel(EvcVerificationViewModel viewModel)
        {
            return VerificationMapper.MapViewModelToModel(viewModel);
        }

        public void Dispose()
        {
            _cleanup?.Dispose();
            _verificationTests?.Dispose();
        }

        public IObservableCache<EvcVerificationTest, Guid> FetchTests()
        {
            return Load();
        }

        public IObservableCache<EvcVerificationTest, Guid> Load()
        {
            if (_verificationTests == null)
            {

                _testsCache.AddOrUpdate(_verificationRepository.Query());

                var changes = _testsCache.AsObservableCache().Connect().Publish();
                _verificationTests = changes.AsObservableCache();

                changes.Connect().DisposeWith(_cleanup);
                _cleanup.Add(_verificationTests);
            }

            return _verificationTests;
        }


        public async Task<EvcVerificationViewModel> GetViewModel(EvcVerificationTest verificationTest)
        {
            return await Task.Run(() => VerificationMapper.MapModelToViewModel(verificationTest));
        }

        public async Task<ICollection<EvcVerificationViewModel>> GetViewModel(
            IEnumerable<EvcVerificationTest> verificationTests)
        {
            var evcTests = new ConcurrentBag<EvcVerificationViewModel>();

            foreach (var model in verificationTests)
                evcTests.Add(await GetViewModel(model));

            return evcTests.ToList();
        }

        public EvcVerificationViewModel NewVerification(DeviceInstance device)
        {
            var testModel = _evcVerificationTestFactory?.Invoke(device) ?? new EvcVerificationTest(device);

            return _verificationViewModelFactory.CreateViewModel(testModel);
        }

        public async Task<ITestManager> NewTestManager(DeviceType deviceType) => await _verificationManagerFactory.StartNew(this, deviceType);

        private IObservable<IChangeSet<EvcVerificationTest, Guid>> GetTests(Expression<Func<EvcVerificationTest, bool>> predicate = null)
        {
            return _verificationRepository.Query(predicate).ToObservable().ToObservableChangeSet(t => t.Id);
            //return ObservableChangeSet.Create<EvcVerificationTest, Guid>(cache =>
            //{
            //    var cleanup = new CompositeDisposable();
                
            //    cache.AddOrUpdate(_verificationRepository.Query(predicate));

            //    Updates.Subscribe(cache.AddOrUpdate);

            //    return cleanup;
            //}, test => test.Id);
        }
    }
}