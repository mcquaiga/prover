using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Screens.Dialogs;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Services;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public interface ITestManagerViewModelFactory
    {
        Task<TestManager> StartNew(DeviceType deviceType, string commPortName, int baudRate,
            string tachPortName);
    }

    public partial class TestManager : ITestManagerViewModelFactory
    {
        private readonly DateTime _dateCreated;

        public TestManager(ILoggerFactory loggerFactory,
            IScreenManager screenManager,
            IDeviceSessionManager deviceManager,
            VerificationViewModelService testViewModelService,
            DialogServiceManager dialogService,
            IVolumeTestManagerFactory volumeTestManagerFactory) : base(screenManager)
        {
            _logger = loggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;

            _screenManager = screenManager;
            _deviceManager = deviceManager;
            _testViewModelService = testViewModelService;
            _dialogService = dialogService;
            _volumeTestManagerFactory = volumeTestManagerFactory;

            _dateCreated = DateTime.Now;

            //this.WhenNavigatingFromObservable()
            //    .Subscribe(async _ =>
            //    {
            //        _logger.LogDebug("Navigating away from Cleaning up.");
            //        await _deviceManager.EndSession();
            //        RunNavigatingAwayActions();
            //    })
            //    .DisposeWith(_cleanup);
        }

        public async Task<TestManager> StartNew(DeviceType deviceType, string commPortName, int baudRate, string tachPortName)
        {
            if (_deviceManager.SessionInProgress) await Reset();

            await _deviceManager.StartSession(deviceType, commPortName, baudRate, null);

            TestViewModel = _testViewModelService.NewTest(_deviceManager.Device);
            TestViewModel.DisposeWith(_cleanup);

            VolumeTestManager =
                _volumeTestManagerFactory.CreateInstance(_deviceManager, TestViewModel.VolumeTest, tachPortName);
            VolumeTestManager.DisposeWith(_cleanup);

            SetupRxUi();

            await _screenManager.ChangeView(this);

            return this;
        }
    }
}