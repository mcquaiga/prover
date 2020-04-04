using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;

namespace Prover.Application.Verifications
{
    public sealed class TestManager : ReactiveObject, ITestManager, IDisposable
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly IDeviceSessionManager _deviceManager;
        private readonly IVerificationActionsExecutioner _actionsExecutioner;
        private readonly ILogger _logger;

        public TestManager(
            ILogger<TestManager> logger,
            IDeviceSessionManager deviceSessionManager,
            EvcVerificationViewModel verificationViewModel,
            IVolumeTestManager volumeTestManager,
            IVerificationActionsExecutioner customActionsExecutioner)
        {
            _logger = logger;
            _deviceManager = deviceSessionManager;
            _actionsExecutioner = customActionsExecutioner;

            TestViewModel = verificationViewModel;
            VolumeTestManager = volumeTestManager;

            DownloadCommand = ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(RunCorrectionTests);
            DownloadCommand.ThrownExceptions.LogErrors("Error downloading items from instrument.").Subscribe();
            DownloadCommand.DisposeWith(_cleanup);
        }

        public EvcVerificationViewModel TestViewModel { get; protected set; }

        public IVolumeTestManager VolumeTestManager { get; protected set; }

        public ReactiveCommand<VerificationTestPointViewModel, Unit> DownloadCommand { get; protected set; }
        
        public void Dispose()
        {
            _logger.LogDebug("Disposing instance.");
            _deviceManager.EndSession();
            _cleanup?.Dispose();
        }

        public async Task RunCorrectionTests(VerificationTestPointViewModel test)
        {
            await LiveReadCoordinator.StartLiveReading(_deviceManager, test,
                async () =>
                {
                    var values = await _deviceManager.DownloadCorrectionItems();

                    //test.UpdateItemValues(values);

                    foreach (var correction in test.GetCorrectionTests())
                    {
                        var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));
                        itemType?.SetValue(correction,
                            _deviceManager.Device.DeviceType.GetGroupValues(values, itemType.PropertyType));
                    }
                }, RxApp.MainThreadScheduler);
        }
    }
}