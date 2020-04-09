using System;
using System.Reactive;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.VerificationManagers
{
    public sealed class RotaryTestManager : TestManagerBase, ITestManager
    {
        private readonly IActionsExecutioner _actionsExecutioner;
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly IDeviceSessionManager _deviceManager;
        private readonly ILogger _logger;

        public RotaryTestManager(
            ILogger<RotaryTestManager> logger,
            IScreenManager screenManager,
            IDeviceSessionManager deviceSessionManager,
            EvcVerificationViewModel verificationViewModel,
            IVerificationTestService verificationService,
            IVolumeTestManager volumeTestManager,
            IActionsExecutioner actionsExecutioner,
            ICorrectionTestsManager correctionVerificationRunner) : base(logger, screenManager, verificationService, verificationViewModel)
        {
            _logger = logger;
            _deviceManager = deviceSessionManager;
            _actionsExecutioner = actionsExecutioner;

            VolumeTestManager = volumeTestManager;
            CorrectionVerifications = correctionVerificationRunner;

            //ExecuteStartActions = ReactiveCommand.CreateFromTask(async () =>
            //    await _actionsExecutioner.RunActionsOn<IInitializeAction>(TestViewModel));

            RunCorrectionVerifications =
                ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(CorrectionVerifications.RunCorrectionTests);
            RunCorrectionVerifications.ThrownExceptions.LogErrors("Error downloading items from instrument.")
                .Subscribe();
            RunCorrectionVerifications.DisposeWith(_cleanup);
        }

        public IVolumeTestManager VolumeTestManager { get; }
        public ICorrectionTestsManager CorrectionVerifications { get; }

        public ReactiveCommand<VerificationTestPointViewModel, Unit> RunCorrectionVerifications { get; }
        public ReactiveCommand<VerificationTestPointViewModel, Unit> RunVolumeVerifications { get; }

        //public ReactiveCommand<Unit, Unit> ExecuteStartActions { get; }

        protected override void Disposing()
        {
            _logger.LogDebug("Disposing instance.");
            _deviceManager.EndSession();
            _cleanup?.Dispose();
        }
    }
}