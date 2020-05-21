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
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Devices.Core.Repository;
using DynamicData;
using Prover.Shared.Extensions;

namespace Prover.Application.Services
{

	public interface IVerificationQueries
	{
		Task<IEnumerable<EvcVerificationTest>> Query(Expression<Func<EvcVerificationTest, bool>> filter);
		Task<IEnumerable<EvcVerificationTest>> Query(DateTime fromDateTime, DateTime? toDateTime = null, bool includeArchived = false, bool includeExported = false);
	}

	public class VerificationService :
			IVerificationService,
			//ICacheAggregateRoot<EvcVerificationTest>,
			IDisposable
	{
		private readonly ILogger<VerificationService> _logger;
		private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;
		//private readonly ICacheAggregateRoot<EvcVerificationTest> _verificationCache;
		private readonly IVerificationManagerService _managerService;
		private readonly IDeviceRepository _deviceRepository;
		private readonly IDeviceSessionManager _deviceManager;

		public VerificationService(
				ILogger<VerificationService> logger,
				IAsyncRepository<EvcVerificationTest> verificationRepository,
				IVerificationManagerService managerService,
				IDeviceRepository deviceRepository,
				IDeviceSessionManager deviceManager,
				Func<DeviceInstance, EvcVerificationTest> evcVerificationTestFactory = null)
		{
			_logger = logger ?? NullLogger<VerificationService>.Instance;

			_verificationRepository = verificationRepository;
			_managerService = managerService;
			_deviceRepository = deviceRepository;
			_deviceManager = deviceManager;

			Query = VerificationQueries.Initialize(_verificationRepository);
		}

		public async Task<EvcVerificationViewModel> Save(EvcVerificationViewModel viewModel)
		{
			var model = await Save(VerificationMapper.MapViewModelToModel(viewModel));
			return model.ToViewModel();
		}

		public async Task<EvcVerificationTest> Save(EvcVerificationTest evcVerificationTest)
		{
			await _verificationRepository.UpsertAsync(evcVerificationTest);
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

		public VerificationQueries Query { get; }

		/// <inheritdoc />
		public async Task<EvcVerificationTest> Archive(EvcVerificationTest model)
		{
			model.ArchivedDateTime = DateTime.Now;
			return await Save(model);
		}

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

			var manager = _managerService.CreateManager(device.NewVerification(options));

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



		///// <inheritdoc />
		//public IObservableCache<EvcVerificationTest, Guid> Data => _verificationCache.Data;

		///// <inheritdoc />
		//public Task Refresh(Expression<Func<EvcVerificationTest, bool>> filter = null) => _verificationCache.Refresh(filter);


	}

	public static class VerificationServiceEx
	{
		public static IObservable<IChangeSet<EvcVerificationTest, Guid>> QueryObservable(this IVerificationService service, DateTime fromDateTime, DateTime? toDateTime = null)
		{
			return ObservableChangeSet.Create<EvcVerificationTest, Guid>(async cache =>
			{
				var loader = await service.Query.TestDateBetween(fromDateTime, toDateTime);

				cache.Edit(updater =>
				{
					loader.ToObservable()
						  .Subscribe(updater.AddOrUpdate);
				});

				return new CompositeDisposable();
			}, test => test.Id);

		}
	}
}