using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Verifications
{
	public class TestManager : TestManagerBase, IQaTestRunManager, ITestManagersProvider
	{
		public IVolumeTestManager VolumeTestManager { get; set; }
		public ICorrectionTestsManager CorrectionVerifications { get; set; }

		public TestManager(ILogger<TestManager> logger, IScreenManager screenManager,
				IDeviceSessionManager deviceManager,
				IVerificationService verificationService) : base(logger, screenManager, verificationService, null) { }

		/// <inheritdoc />
		public TestManager(ILogger<TestManager> logger, IScreenManager screenManager,
				IDeviceSessionManager deviceManager,
				IVerificationService verificationService,
				IVolumeTestManager volumeTestManager,
				ICorrectionTestsManager correctionVerificationRunner, EvcVerificationViewModel verificationViewModel = null)
				: base(logger, screenManager, verificationService, verificationViewModel)
		{
			DeviceManager = deviceManager;

			VolumeTestManager = volumeTestManager;
			CorrectionVerifications = correctionVerificationRunner;

			RunCorrectionVerifications =
					ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(CorrectionVerifications.RunCorrectionTests).DisposeWith(Cleanup);
			ObservableLoggingExtensions.LogErrors(RunCorrectionVerifications.ThrownExceptions, "Error downloading items from instrument.")
									   .Subscribe().DisposeWith(Cleanup);

			RunVolumeVerifications = ReactiveCommand.CreateFromTask(VolumeTestManager.BeginVolumeVerification).DisposeWith(Cleanup);
		}

		public IDeviceSessionManager DeviceManager { get; }

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				DeviceManager?.EndSession();
				base.Dispose(true);
			}

		}



		public ReactiveCommand<VerificationTestPointViewModel, Unit> RunCorrectionVerifications { get; set; }
		public ReactiveCommand<Unit, Unit> RunVolumeVerifications { get; set; }

		/// <inheritdoc />
		public Task CompleteVolumeVerification() => throw new NotImplementedException();

		/// <inheritdoc />
		public Task BeginVolumeVerification() => throw new NotImplementedException();
	}
}