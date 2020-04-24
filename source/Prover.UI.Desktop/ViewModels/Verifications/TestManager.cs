using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Verifications;
using Prover.Application.ViewModels;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;

namespace Prover.UI.Desktop.ViewModels.Verifications
{
    public sealed class TestManager : TestManagerBase, IDeviceQaTestManager, IRoutableViewModel
    {

        private readonly IScreenManager _screenManager;
        public IDeviceSessionManager DeviceManager { get; }
        private readonly IVerificationTestService _verificationService;
        private readonly ILogger<TestManager> _logger;

        public TestManager(
                ILogger<TestManager> logger,
                IScreenManager screenManager,
                IDeviceSessionManager deviceSessionManager,
                IVerificationTestService verificationService)
        {
            _logger = logger ?? NullLogger<TestManager>.Instance;
            _screenManager = screenManager;
            DeviceManager = deviceSessionManager;
            _verificationService = verificationService;
        }

        public IVolumeTestManager VolumeTestManager { get; set; }
        public ICorrectionTestsManager CorrectionVerifications { get; set; }

        public ReactiveCommand<VerificationTestPointViewModel, Unit> RunCorrectionVerifications { get; set; }
        public ReactiveCommand<Unit, Unit> RunVolumeVerifications { get; set; }

        public void Setup(EvcVerificationViewModel verificationViewModel, IVolumeTestManager volumeTestManager, ICorrectionTestsManager correctionVerificationRunner)
        {
            base.Initialize(_logger, _screenManager, _verificationService, verificationViewModel);

            VolumeTestManager = volumeTestManager;
            CorrectionVerifications = correctionVerificationRunner;

            RunCorrectionVerifications =
                    ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(CorrectionVerifications.RunCorrectionTests).DisposeWith(Cleanup);
            RunCorrectionVerifications.ThrownExceptions.LogErrors("Error downloading items from instrument.")
                                      .Subscribe().DisposeWith(Cleanup);

            RunVolumeVerifications = ReactiveCommand.CreateFromTask(VolumeTestManager.RunStartActions).DisposeWith(Cleanup);
        }

        public void StartTest(DeviceType deviceType)
        {

        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                DeviceManager.EndSession();
                TestViewModel?.Dispose();
            }

        }

        /// <inheritdoc />
        public string UrlPathSegment { get; }

        /// <inheritdoc />
        public IScreen HostScreen => _screenManager;
    }
}