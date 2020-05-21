using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.ViewModels;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels.Factories;

namespace Prover.Application.Verifications.Factories
{
	public class TestManagerOptions
	{

	}

	public class VerificationManagerService : IVerificationManagerService
	{
		private readonly ILogger<VerificationManagerService> _logger;
		//private IDeviceSessionManager _deviceManager;
		private Func<EvcVerificationViewModel, IQaTestRunManager> _testManagerFactory;
		private readonly TestManagerOptions _options;

		public VerificationManagerService(
				ILogger<VerificationManagerService> logger,
				//IDeviceSessionManager deviceManager,
				Func<EvcVerificationViewModel, IQaTestRunManager> testManagerFactory,
				TestManagerOptions options = null)
		{
			_logger = logger;
			//_deviceManager = deviceManager;
			_testManagerFactory = testManagerFactory;
			_options = options;
		}

		public IQaTestRunManager CreateManager(EvcVerificationTest verificationTest)
		{
			var viewModel = verificationTest.ToViewModel();

			return _testManagerFactory(viewModel);
		}

		///// <inheritdoc />
		//public async Task<bool> CompleteVerification(EvcVerificationViewModel viewModel)
		//{
		//	if (viewModel.SubmittedDateTime != null)
		//		throw new NotSupportedException("This test has already been submitted");

		//	viewModel.SubmittedDateTime = DateTime.Now;

		//	_logger.LogDebug($"Submitting verification Id: {viewModel.Id} at {viewModel.SubmittedDateTime:g}");

		//	var model = await Save(viewModel.ToModel());

		//	await VerificationEvents.OnSubmit.Publish(model);

		//	return model != null;
		//}


	}

	public static class ManagerFactoryMixins
	{
		//public static async Task<IQaTestRunManager> CreateTestManager(this DeviceInstance deviceInstance)
		//{
		//	return await VerificationManagerService.CreateManager(deviceInstance.DeviceType);
		//}
	}
}