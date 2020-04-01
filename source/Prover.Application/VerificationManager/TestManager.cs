using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;

namespace Prover.Application.VerificationManager
{
    public class VerificationTestManagerFactory : ITestManagerFactory
    {
        private readonly ILogger<VerificationTestManagerFactory> _logger;
        private readonly IDeviceSessionManager _deviceSessionManager;
        private readonly Func<EvcVerificationViewModel, IVolumeTestManager> _volumeTestManagerFactory;
        private readonly IDeviceVerificationValidator _deviceValidator;
        private readonly Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager> _testManagerFactory;

        public VerificationTestManagerFactory(ILogger<VerificationTestManagerFactory> logger,
            IDeviceSessionManager deviceSessionManager,
            Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager> testManagerFactory,
            Func<EvcVerificationViewModel, IVolumeTestManager> volumeTestManagerFactory,
            IDeviceVerificationValidator deviceValidator)
        {
            _logger = logger;
            _deviceSessionManager = deviceSessionManager;
            _testManagerFactory = testManagerFactory;
            _volumeTestManagerFactory = volumeTestManagerFactory;
            _deviceValidator = deviceValidator;
        }

        public async Task<ITestManager> StartNew(VerificationTestService verificationService, DeviceType deviceType)
        {
            var device = await _deviceSessionManager.StartSession(deviceType);
            await _deviceValidator.RunValidations(device);

            var testViewModel = verificationService.NewTest(_deviceSessionManager.Device);
            var volumeManager = _volumeTestManagerFactory.Invoke(testViewModel);
            return _testManagerFactory.Invoke(testViewModel, volumeManager);
        }
    }

    public class TestManager : ReactiveObject, ITestManager, IDisposable
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly IDeviceSessionManager _deviceManager;
        private readonly IDeviceVerificationValidator _deviceValidator;
        private readonly ILogger _logger;

        public TestManager(
            ILogger<TestManager> logger,
            IDeviceSessionManager deviceSessionManager,
            EvcVerificationViewModel verificationViewModel,
            IVolumeTestManager volumeTestManager,
            IDeviceVerificationValidator deviceValidator)
        {
            TestViewModel = verificationViewModel;
            _deviceManager = deviceSessionManager;
            _deviceValidator = deviceValidator;

            _logger = logger;

            VolumeTestManager = volumeTestManager;

            SetupRxUi();
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