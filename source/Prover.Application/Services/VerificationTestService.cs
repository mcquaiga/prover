using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    public class VerificationTestService : IVerificationTestService
    {
        private readonly IDeviceSessionManager _deviceManager;

        private readonly Func<DeviceInstance, EvcVerificationTest> _evcVerificationTestFactory =
            device => new EvcVerificationTest(device);

        private readonly ISourceCache<EvcVerificationViewModel, Guid> _testsCache =
            new SourceCache<EvcVerificationViewModel, Guid>(k => k.Id);

        private readonly IAsyncRepository<EvcVerificationTest> _testService;
        private readonly ITestManagerFactory _verificationManagerFactory;
        private readonly IVerificationViewModelFactory _verificationViewModelFactory;

        public VerificationTestService(
            IAsyncRepository<EvcVerificationTest> verificationRepository,
            IVerificationViewModelFactory verificationViewModelFactory,
            IDeviceSessionManager deviceManager,
            ITestManagerFactory verificationManagerFactory,
            Func<DeviceInstance, EvcVerificationTest> evcVerificationTestFactory = null)
        {
            _testService = verificationRepository;
            _verificationViewModelFactory = verificationViewModelFactory;
            _deviceManager = deviceManager;
            _verificationManagerFactory = verificationManagerFactory;
            _evcVerificationTestFactory = evcVerificationTestFactory ?? _evcVerificationTestFactory;
            _testsCache.Connect();
        }

        public async Task<bool> AddOrUpdate(EvcVerificationViewModel viewModel)
        {
            var model = VerificationMapper.MapViewModelToModel(viewModel);

            await _testService.AddAsync(model);

            return true;
        }

        public async Task<EvcVerificationTest> AddOrUpdate(EvcVerificationTest evcVerificationTest)
        {
            await _testService.AddAsync(evcVerificationTest);

            return evcVerificationTest;
        }

        public EvcVerificationTest CreateVerificationTestFromViewModel(EvcVerificationViewModel viewModel)
            => VerificationMapper.MapViewModelToModel(viewModel);

        public async Task<EvcVerificationViewModel> GetVerificationTest(EvcVerificationTest verificationTest)
            => await Task.Run(() => VerificationMapper.MapModelToViewModel(verificationTest));

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
            var testModel = _evcVerificationTestFactory.Invoke(device);

            return _verificationViewModelFactory.CreateViewModel(testModel);
        }

        public async Task<ITestManager> NewTestManager(DeviceType deviceType)
            => await _verificationManagerFactory.StartNew(this, deviceType);
    }
}