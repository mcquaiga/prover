using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.VerificationManager.Volume;

namespace Prover.Application.VerificationManager
{
    public class TestManagerFactory : ITestManagerFactory
    {
        private readonly IVolumeTestManagerFactory _volumeTestManagerFactory;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly ILogger _logger;
        private readonly IScreenManager _screenManager;

        public TestManagerFactory(
            ILoggerFactory loggerFactory,
            IScreenManager screenManager,
            IDeviceSessionManager deviceManager,
            IVolumeTestManagerFactory volumeTestManagerFactory)
        {
            _logger = loggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;

            _screenManager = screenManager;
            _deviceManager = deviceManager;
            _volumeTestManagerFactory = volumeTestManagerFactory;
        }

        public async Task<ITestManager> StartNew(DeviceType deviceType, string commPortName, int baudRate,
            string tachPortName)
        {
            //if (_deviceManager.SessionInProgress) throw new Exception("Device session in progress. End session before creating new test.");

            //await _deviceManager.StartSession(deviceType, commPortName, baudRate, null);

            //var testViewModel = NewTest(_deviceManager.Device);

            //var volumeManager =
            //    _volumeTestManagerFactory.CreateVolumeManager(_deviceManager, testViewModel);
            //(VolumeTestManager as IDisposable)?.DisposeWith(_cleanup);

            //SetupRxUi();

            //await _screenManager.ChangeView(this);

            return null;
        }
    }

    //public partial class TestManager : ITestManagerFactory, ITestManager
    //{
        
    //    private readonly DateTime _dateCreated;
    //    private readonly IVolumeTestManagerFactory _volumeTestManagerFactory;

    //    public TestManager(
    //        ILoggerFactory loggerFactory,
    //        IScreenManager screenManager,
    //        IDeviceSessionManager deviceManager,
    //        VerificationTestService testViewModelService,
    //        IVolumeTestManagerFactory volumeTestManagerFactory) : base(screenManager)
    //    {
    //        _logger = loggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;

    //        _screenManager = screenManager;
    //        _deviceManager = deviceManager;
    //        _testViewModelService = testViewModelService;
    //        _volumeTestManagerFactory = volumeTestManagerFactory;

    //        _dateCreated = DateTime.Now;
    //    }

    //    public TestManager(
    //        ILogger<TestManager> logger,
    //        IDeviceSessionManager deviceSessionManager,
    //        IScreenManager screenManager,
    //        EvcVerificationViewModel verificationViewModel,
    //        IVolumeTestManager volumeTestManager) : base(screenManager)
    //    {
    //        _verificationViewModel = verificationViewModel;
    //        VolumeTestManager = volumeTestManager;
    //        _deviceManager = deviceSessionManager;
    //        _logger = logger;
    //    }

    //    public async Task<ITestManager> StartNew(DeviceType deviceType, string commPortName, int baudRate,
    //        string tachPortName)
    //    {
    //        if (_deviceManager.SessionInProgress) await Reset();

    //        await _deviceManager.StartSession(deviceType, commPortName, baudRate, null);

    //        TestViewModel = _testViewModelService.NewTest(_deviceManager.Device);
    //        TestViewModel.DisposeWith(_cleanup);

    //        VolumeTestManager =
    //            _volumeTestManagerFactory.CreateVolumeManager(_deviceManager, TestViewModel);
    //        (VolumeTestManager as IDisposable)?.DisposeWith(_cleanup);

    //        SetupRxUi();

    //        await _screenManager.ChangeView(this);

    //        return this;
    //    }
    //}
}