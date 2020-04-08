using System;
using System.Reactive;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Verifications
{
    public sealed class TestManager : ReactiveObject, ITestManager, IDisposable
    {
        private readonly IActionsExecutioner _actionsExecutioner;
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly IDeviceSessionManager _deviceManager;
        private readonly ILogger _logger;

        public TestManager(
            ILogger<TestManager> logger,
            IDeviceSessionManager deviceSessionManager,
            EvcVerificationViewModel verificationViewModel,
            IVolumeTestManager volumeTestManager,
            IActionsExecutioner actionsExecutioner,
            ICorrectionVerificationRunner correctionVerificationRunner)
        {
            _logger = logger;
            _deviceManager = deviceSessionManager;
            _actionsExecutioner = actionsExecutioner;

            TestViewModel = verificationViewModel;
            VolumeTestManager = volumeTestManager;
            CorrectionVerifications = correctionVerificationRunner;

            ExecuteStartActions = ReactiveCommand.CreateFromTask(async () =>
                await _actionsExecutioner.RunActionsOn<IInitializeAction>(TestViewModel));

            RunCorrectionVerifications =
                ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(CorrectionVerifications.RunCorrectionTests);
            RunCorrectionVerifications.ThrownExceptions.LogErrors("Error downloading items from instrument.")
                .Subscribe();
            RunCorrectionVerifications.DisposeWith(_cleanup);
        }

        public EvcVerificationViewModel TestViewModel { get; }

        public IVolumeTestManager VolumeTestManager { get; }

        public ICorrectionVerificationRunner CorrectionVerifications { get; }

        public ReactiveCommand<VerificationTestPointViewModel, Unit> RunCorrectionVerifications { get; }

        public ReactiveCommand<Unit, Unit> ExecuteStartActions { get; }

        public void Dispose()
        {
            _logger.LogDebug("Disposing instance.");
            _deviceManager.EndSession();
            _cleanup?.Dispose();
        }
    }
}