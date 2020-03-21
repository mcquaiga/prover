using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.ViewModels.Dialogs;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.Services;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public interface ITestManagerViewModelFactory
    {
        Task<TestManager> StartNew(DeviceType deviceType, string commPortName, int baudRate,
            string tachPortName);
    }

    public partial class TestManager : ITestManagerViewModelFactory
    {
        public DeviceSessionDialogManager DeviceInteractions { get; }
        private readonly DateTime _dateCreated;
        private readonly IVolumeTestManagerFactory _volumeTestManagerFactory;

        public TestManager(
            ILoggerFactory loggerFactory,
            IScreenManager screenManager,
            IDeviceSessionManager deviceManager,
            VerificationViewModelService testViewModelService,
            IVolumeTestManagerFactory volumeTestManagerFactory,
            DeviceSessionDialogManager deviceInteractions) : base(screenManager)
        {
            DeviceInteractions = deviceInteractions;
            _logger = loggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;

            _screenManager = screenManager;
            _deviceManager = deviceManager;
            _testViewModelService = testViewModelService;
            _volumeTestManagerFactory = volumeTestManagerFactory;

            _dateCreated = DateTime.Now;
        }

        public async Task<TestManager> StartNew(DeviceType deviceType, string commPortName, int baudRate, string tachPortName)
        {
            if (_deviceManager.SessionInProgress) await Reset();

            await _deviceManager.StartSession(deviceType, commPortName, baudRate, null);

            TestViewModel = _testViewModelService.NewTest(_deviceManager.Device);
            TestViewModel.DisposeWith(_cleanup);

            RotaryVolumeTestManager = _volumeTestManagerFactory.CreateInstance(_deviceManager, TestViewModel.VolumeTest, tachPortName);
            RotaryVolumeTestManager.DisposeWith(_cleanup);

            SetupRxUi();

            await _screenManager.ChangeView(this);

            return this;
        }
    }
}