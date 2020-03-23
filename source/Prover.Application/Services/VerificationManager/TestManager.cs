using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Prover.Application.Extensions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;

namespace Prover.Application.Services.VerificationManager
{
    public class TestManager : ReactiveObject, ITestManager, IDisposable
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly IDeviceSessionManager _deviceManager;
        private readonly ILogger _logger;

        public TestManager(
            ILogger<TestManager> logger,
            IDeviceSessionManager deviceSessionManager,
            EvcVerificationViewModel verificationViewModel,
            Func<EvcVerificationViewModel, IVolumeTestManager> volumeTestManagerFactory)
        {
            TestViewModel = verificationViewModel;
            _deviceManager = deviceSessionManager;
            _logger = logger;

            VolumeTestManager = volumeTestManagerFactory.Invoke(verificationViewModel);

            SetupRxUi();
        }

        public EvcVerificationViewModel TestViewModel { get; protected set; }

        public IVolumeTestManager VolumeTestManager { get; protected set; }

        public ReactiveCommand<VerificationTestPointViewModel, Unit> DownloadCommand { get; protected set; }

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

        public void Dispose()
        {
            _logger.LogDebug("Disposing instance.");
            _deviceManager.EndSession();
            _cleanup?.Dispose();
        }

        protected virtual void ExecuteSubmitActions()
        {
            // Ask if they want to save
        }

        protected void SetupRxUi()
        {
            DownloadCommand = ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(RunCorrectionTests);
            DownloadCommand.ThrownExceptions.LogErrors("Error downloading items from instrument.").Subscribe();
            DownloadCommand.DisposeWith(_cleanup);
        }

        private async Task Reset()
        {
            await _deviceManager.EndSession();
        }

       
    }
}