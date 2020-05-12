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
	public class VerificationService : IVerificationService, IDisposable
	{
		private readonly Func<DeviceInstance, EvcVerificationTest> _evcVerificationTestFactory;
		private readonly ILogger<VerificationService> _logger;
		private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;
		private readonly IEntityDataCache<EvcVerificationTest> _verificationCache;
		private readonly IVerificationManagerService _managerService;
		private readonly IDeviceSessionManager _deviceManager;

		public VerificationService(
				ILogger<VerificationService> logger,
				IAsyncRepository<EvcVerificationTest> verificationRepository,
				IEntityDataCache<EvcVerificationTest> verificationCache,
				IVerificationManagerService managerService,
				IDeviceSessionManager deviceManager,
				Func<DeviceInstance, EvcVerificationTest> evcVerificationTestFactory = null,
				IScheduler scheduler = null)
		{
			_logger = logger ?? NullLogger<VerificationService>.Instance;

			_verificationRepository = verificationRepository;
			_verificationCache = verificationCache;

			_managerService = managerService;
			_deviceManager = deviceManager;
			_evcVerificationTestFactory = evcVerificationTestFactory;
		}

		public async Task<EvcVerificationViewModel> Save(EvcVerificationViewModel viewModel)
		{
			var model = await Save(VerificationMapper.MapViewModelToModel(viewModel));
			return model.ToViewModel();
		}

		public async Task<EvcVerificationTest> Save(EvcVerificationTest evcVerificationTest)
		{
			await _verificationRepository.UpsertAsync(evcVerificationTest);

			await VerificationEvents.OnSave.Publish(evcVerificationTest);

			return evcVerificationTest;
		}

		public async Task Save(IEnumerable<EvcVerificationTest> evcVerificationTest)
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
			return await Save(model);
		}

		public IVerificationManagerService ManagerService => _managerService;

		public async Task<IQaTestRunManager> StartVerification(DeviceType deviceType, VerificationTestOptions options = null)
		{
			var device = await _deviceManager.StartSession(deviceType);

			var manager = await StartVerification(device, options, true);

			await _deviceManager.Disconnect();

			return manager;
		}

		public async Task<IQaTestRunManager> StartVerification(DeviceInstance device, VerificationTestOptions options = null, bool publishEvent = false)
		{
			options = options ?? VerificationTestOptions.Defaults;

			var manager = _managerService.CreateManager(
										device.NewVerification(options));

			if (publishEvent)
				await VerificationEvents.OnInitialize.Publish(manager.TestViewModel);

			return manager;
		}

		public async Task<bool> CompleteVerification(EvcVerificationViewModel viewModel)
		{
			if (viewModel.SubmittedDateTime != null)
				throw new NotSupportedException("This test has already been submitted");

			viewModel.SubmittedDateTime = DateTime.Now;

			_logger.LogDebug($"Submitting verification Id: {viewModel.Id} at {viewModel.SubmittedDateTime:g}");

			var model = await Save(viewModel.ToModel());

			await VerificationEvents.OnSubmit.Publish(model);

			return model != null;
		}

		/// <inheritdoc />
		public void Dispose()
		{
		}
	}
}