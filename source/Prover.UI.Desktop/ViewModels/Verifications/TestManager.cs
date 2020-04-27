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
    public class ManualTestManager : TestManagerBase, IRoutableViewModel
    {
        private readonly IScreenManager _screenManager;

        private readonly IVerificationTestService _verificationService;
        private readonly ILogger<ManualTestManager> _logger;

        public ManualTestManager(
                ILogger<ManualTestManager> logger,
                IScreenManager screenManager,
                IVerificationTestService verificationService,
                EvcVerificationViewModel verificationViewModel = null) : base(logger, screenManager, verificationService, verificationViewModel)
        {
            _logger = logger ?? NullLogger<ManualTestManager>.Instance;
            _screenManager = screenManager;
            _verificationService = verificationService;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                TestViewModel?.Dispose();
            }

        }

        /// <inheritdoc />
        public string UrlPathSegment { get; }

        /// <inheritdoc />
        public IScreen HostScreen => _screenManager;
    }

    public class TestManager : ManualTestManager, IDeviceQaTestManager
    {

        public IVolumeTestManager VolumeTestManager { get; set; }
        public ICorrectionTestsManager CorrectionVerifications { get; set; }
        /// <inheritdoc />
        public TestManager(ILogger<TestManager> logger, IScreenManager screenManager, IDeviceSessionManager deviceManager, IVerificationTestService verificationService,
                EvcVerificationViewModel verificationViewModel = null,
                IVolumeTestManager volumeTestManager = null, ICorrectionTestsManager correctionVerificationRunner = null)
                : base(logger, screenManager, verificationService, verificationViewModel)
        {
            DeviceManager = deviceManager;

            VolumeTestManager = volumeTestManager;
            CorrectionVerifications = correctionVerificationRunner;

            RunCorrectionVerifications =
                    ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(CorrectionVerifications.RunCorrectionTests).DisposeWith(Cleanup);
            RunCorrectionVerifications.ThrownExceptions.LogErrors("Error downloading items from instrument.")
                                      .Subscribe().DisposeWith(Cleanup);

            RunVolumeVerifications = ReactiveCommand.CreateFromTask(VolumeTestManager.RunStartActions).DisposeWith(Cleanup);
        }

        public IDeviceSessionManager DeviceManager { get; }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                DeviceManager.EndSession();
                base.Dispose(true);
            }

        }



        public ReactiveCommand<VerificationTestPointViewModel, Unit> RunCorrectionVerifications { get; set; }
        public ReactiveCommand<Unit, Unit> RunVolumeVerifications { get; set; }
    }
}