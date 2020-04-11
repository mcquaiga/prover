using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Factories;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;

namespace Prover.Application.Services
{
    public partial class VerificationTestService : IVerificationTestService, IDisposable
    {
        private readonly Func<DeviceInstance, EvcVerificationTest> _evcVerificationTestFactory;
        private readonly object _lock = new AsyncLock();
        private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;
        private readonly IVerificationViewModelFactory _verificationViewModelFactory;

        public VerificationTestService(
                IAsyncRepository<EvcVerificationTest> verificationRepository,
                IVerificationViewModelFactory verificationViewModelFactory,
                Func<DeviceInstance, EvcVerificationTest> evcVerificationTestFactory = null,
                IScheduler scheduler = null)
        {
            _verificationRepository = verificationRepository;
            _verificationViewModelFactory = verificationViewModelFactory;
            _evcVerificationTestFactory = evcVerificationTestFactory;

            SetupCache();
        }

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

            _cacheUpdates.AddOrUpdate(evcVerificationTest);

            return evcVerificationTest;
        }

        public EvcVerificationTest CreateModel(EvcVerificationViewModel viewModel) => VerificationMapper.MapViewModelToModel(viewModel);

        public void Dispose()
        {
            _cleanup?.Dispose();
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
    }
}