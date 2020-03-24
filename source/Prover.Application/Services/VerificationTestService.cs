using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume.Factories;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;

namespace Prover.Application.Services
{
    public class VerificationTestService
    {
        private readonly ISourceCache<EvcVerificationViewModel, Guid> _testsCache =
            new SourceCache<EvcVerificationViewModel, Guid>(k => k.Id);

        private readonly IAsyncRepository<EvcVerificationTest> _testService;
        private readonly IVerificationViewModelFactory _verificationViewModelFactory;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly Func<EvcVerificationViewModel, VerificationTestService, ITestManager> _testManagerFactory;

        public VerificationTestService(IAsyncRepository<EvcVerificationTest> verificationRepository, 
            IVerificationViewModelFactory verificationViewModelFactory,
            IDeviceSessionManager deviceManager,
            Func<EvcVerificationViewModel, VerificationTestService, ITestManager> testManagerFactory)
        {
            _testService = verificationRepository;
            _verificationViewModelFactory = verificationViewModelFactory;
            _deviceManager = deviceManager;
            _testManagerFactory = testManagerFactory;

            _testsCache.Connect();
        }

        public async Task<bool> AddOrUpdate(EvcVerificationViewModel viewModel)
        {
            var model = VerificationMapper.MapViewModelToModel(viewModel);

            await _testService.AddAsync(model);

            return true;
        }

        public EvcVerificationTest CreateVerificationTestFromViewModel(EvcVerificationViewModel viewModel) =>
            VerificationMapper.MapViewModelToModel(viewModel);
        
        public async Task<EvcVerificationViewModel> GetVerificationTest(
            EvcVerificationTest verificationTest)
        {
            return await Task.Run(() => VerificationMapper.MapModelToViewModel(verificationTest));
        }

        public async Task<ICollection<EvcVerificationViewModel>> GetVerificationTests(
            IEnumerable<EvcVerificationTest> verificationTests)
        {
            var evcTests = new ConcurrentBag<EvcVerificationViewModel>();

            foreach (var model in verificationTests) 
                evcTests.Add(await GetVerificationTest(model));

            return evcTests.ToList();
        }

        public EvcVerificationViewModel NewTest(DeviceInstance device)
        {
            var testModel = new EvcVerificationTest(device);

            return _verificationViewModelFactory.CreateViewModel(testModel);
        }

        public async Task<ITestManager> NewTestManager(DeviceType deviceType)
        {
            //if (_deviceManager.SessionInProgress) throw new Exception("Device session in progress. End session before creating new test.");

            await _deviceManager.StartSession(deviceType);

            var testViewModel = NewTest(_deviceManager.Device);

            return _testManagerFactory.Invoke(testViewModel, this);
        }
    }
}