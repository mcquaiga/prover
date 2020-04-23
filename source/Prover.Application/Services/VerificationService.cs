using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.Verifications;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Factories;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Prover.Application.Services
{
    public class VerificationService : IVerificationTestService, IDisposable
    {
        private readonly Func<DeviceInstance, EvcVerificationTest> _evcVerificationTestFactory;
        private readonly ILogger<VerificationService> _logger;
        private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;
        private readonly IEntityDataCache<EvcVerificationTest> _verificationCache;
        private readonly IVerificationViewModelFactory _verificationViewModelFactory;

        public VerificationService(
                ILogger<VerificationService> logger,
                IAsyncRepository<EvcVerificationTest> verificationRepository,
                IEntityDataCache<EvcVerificationTest> verificationCache,
                IVerificationViewModelFactory verificationViewModelFactory,
                Func<DeviceInstance, EvcVerificationTest> evcVerificationTestFactory = null,
                IScheduler scheduler = null)
        {
            _logger = logger ?? NullLogger<VerificationService>.Instance;

            _verificationRepository = verificationRepository;
            _verificationCache = verificationCache;
            _verificationViewModelFactory = verificationViewModelFactory;

            _evcVerificationTestFactory = evcVerificationTestFactory;
        }

        public async Task<EvcVerificationViewModel> Save(EvcVerificationViewModel viewModel)
        {
            var model = await Upsert(VerificationMapper.MapViewModelToModel(viewModel));
            return model.ToViewModel();
        }

        public async Task<EvcVerificationTest> Upsert(EvcVerificationTest evcVerificationTest)
        {
            await _verificationRepository.UpsertAsync(evcVerificationTest);

            await VerificationEvents.OnSave.Publish(evcVerificationTest);

            return evcVerificationTest;
        }

        public async Task UpsertBatch(IEnumerable<EvcVerificationTest> evcVerificationTest)
        {
            await evcVerificationTest.ToObservable()
                                     .ForEachAsync(async test =>
                                     {
                                         await _verificationRepository.UpsertAsync(test);
                                     });

            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<EvcVerificationTest> Archive(EvcVerificationTest model)
        {
            model.ArchivedDateTime = DateTime.Now;
            return await Upsert(model);
        }

        public EvcVerificationTest CreateModel(EvcVerificationViewModel viewModel) => VerificationMapper.MapViewModelToModel(viewModel);

        public EvcVerificationViewModel NewVerification(DeviceInstance device, VerificationTestOptions options = null)
        {
            options = options ?? VerificationTestOptions.Defaults;
            //var testModel = _evcVerificationTestFactory?.Invoke(device) ?? new EvcVerificationTest(device);
            var testModel = device.NewVerification(options);
            return testModel.ToViewModel();
        }

        public async Task<EvcVerificationTest> SubmitVerification(EvcVerificationViewModel viewModel)
        {
            if (viewModel.SubmittedDateTime != null) throw new NotSupportedException("This test has already been submitted");

            viewModel.SubmittedDateTime = DateTime.Now;

            var model = viewModel.ToModel();

            await VerificationEvents.OnSubmit.Publish(model);

            model = await Upsert(model);

            return model;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}