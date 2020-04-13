using System;
using System.Reactive;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Verifications;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public sealed class RotaryTestManager : TestManagerBase, ITestManager, IRoutableViewModel
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly IScreenManager _screenManager;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly IVerificationTestService _verificationService;
        private readonly ILogger<RotaryTestManager> _logger;

        public RotaryTestManager(
                ILogger<RotaryTestManager> logger,
                IScreenManager screenManager,
                IDeviceSessionManager deviceSessionManager,
                IVerificationTestService verificationService
        ) 
        {
            _logger = logger ?? NullLogger<RotaryTestManager>.Instance;
            _screenManager = screenManager;
            _deviceManager = deviceSessionManager;
            _verificationService = verificationService;
        }

        public IVolumeTestManager VolumeTestManager { get; set; }
        public ICorrectionTestsManager CorrectionVerifications { get; set; }

        public ReactiveCommand<VerificationTestPointViewModel, Unit> RunCorrectionVerifications { get; set; }
        public ReactiveCommand<VerificationTestPointViewModel, Unit> RunVolumeVerifications { get; }

        //private RotaryTestManager(
        //    ILogger<RotaryTestManager> logger,
        //    IScreenManager screenManager,
        //    IDeviceSessionManager deviceSessionManager,
        //    IVerificationTestService verificationService,
        //    IVolumeTestManager volumeTestManager,
        //    ICorrectionTestsManager correctionVerificationRunner,
        //    EvcVerificationViewModel verificationViewModel)
        //        : base(logger, screenManager, verificationService, verificationViewModel)
        //{


        //}

        public void Setup(EvcVerificationViewModel verificationViewModel, IVolumeTestManager volumeTestManager, ICorrectionTestsManager correctionVerificationRunner)
        {
            base.Initialize(_logger, _screenManager, _verificationService, verificationViewModel);
            
            VolumeTestManager = volumeTestManager;
            CorrectionVerifications = correctionVerificationRunner;

            RunCorrectionVerifications =
                    ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(CorrectionVerifications.RunCorrectionTests);
            RunCorrectionVerifications.ThrownExceptions.LogErrors("Error downloading items from instrument.")
                                      .Subscribe();
            RunCorrectionVerifications.DisposeWith(_cleanup);
        }

        //public ReactiveCommand<Unit, Unit> ExecuteStartActions { get; }

        protected override void Disposing()
        {
            _logger.LogDebug("Disposing instance.");
            _deviceManager.EndSession();
            _cleanup?.Dispose();
        }

        /// <inheritdoc />
        public string UrlPathSegment { get; }

        /// <inheritdoc />
        public IScreen HostScreen => _screenManager;
    }
}