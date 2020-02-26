using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Screens.Dialogs;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Extensions;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using Prover.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public interface ITestManagerViewModelFactory
    {
        Task<TestManagerViewModel> StartNew(DeviceType deviceType, string commPortName, int baudRate,
            string tachPortName);
    }

    //public class TestManagerViewModelFactory : ITestManagerViewModelFactory
    //{
    //    private readonly DeviceSessionManager _deviceManager;
    //    private readonly DialogServiceManager _dialogService;
    //    private readonly ILoggerFactory _loggerFactory;
    //    private readonly IScreenManager _screenManager;
    //    private readonly VerificationViewModelService _testViewModelService;
    //    private readonly IVolumeTestManagerFactory _volumeTestManagerFactory;

    //    public TestManagerViewModelFactory(
    //        ILoggerFactory loggerFactory,
    //        IScreenManager screenManager,
    //        DeviceSessionManager deviceManager,
    //        VerificationViewModelService testViewModelService,
    //        DialogServiceManager dialogService,
    //        IVolumeTestManagerFactory volumeTestManagerFactory)
    //    {
    //        _loggerFactory = loggerFactory;
    //        _screenManager = screenManager;
    //        _deviceManager = deviceManager;
    //        _testViewModelService = testViewModelService;
    //        _dialogService = dialogService;
    //        _volumeTestManagerFactory = volumeTestManagerFactory;
    //    }

    //    public async Task<TestManagerViewModel> StartNew(DeviceType deviceType, string commPortName, int baudRate,
    //        string tachPortName)
    //    {
    //        await _deviceManager.StartSession(deviceType, commPortName, baudRate, null);
    //        var testViewModel = _testViewModelService.NewTest(_deviceManager.Device);
    //        var volumeTestManager =
    //            _volumeTestManagerFactory.CreateInstance(_deviceManager.Device, testViewModel.VolumeTest);

    //        return new TestManagerViewModel(_loggerFactory.CreateLogger(nameof(TestManagerViewModel)), _screenManager, _deviceManager, _testViewModelService,
    //            _dialogService, testViewModel, volumeTestManager);
    //    }
    //}

    public partial class TestManagerViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly DeviceSessionManager _deviceManager;
        private readonly DialogServiceManager _dialogService;
        private readonly IVolumeTestManagerFactory _volumeTestManagerFactory;
        private readonly ILogger _logger;
        private readonly IScreenManager _screenManager;
        private readonly VerificationViewModelService _testViewModelService;

        [Reactive] public EvcVerificationViewModel TestViewModel { get; protected set; }

        [Reactive] public VolumeTestManager VolumeTestManager { get; protected set; }

        public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
        public ReactiveCommand<VerificationTestPointViewModel, Unit> DownloadCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
        public ReactiveCommand<Unit, Unit> RunVolumeTest { get; protected set; }

        public string UrlPathSegment => "/VerificationTests/Details";
        public IScreen HostScreen => _screenManager;

        public async Task Complete()
        {
            //await SaveCurrentState();
        }

        public async Task DownloadItems(VerificationTestPointViewModel test)
        {
            var toDownload = GetItemsToDownload(_deviceManager.Device.Composition());

            var values = await _deviceManager.DownloadCorrectionItems(toDownload);

            test.UpdateItemValues(values);

            foreach (var correction in test.GetCorrectionTests())
            {
                var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));
                itemType?.SetValue(correction,
                    _deviceManager.DeviceType.GetGroupValues(values, itemType.PropertyType));
            }
        }

        private ICollection<ItemMetadata> GetItemsToDownload(CompositionType compType)
        {
            var items = new List<ItemMetadata>();

            if (compType == CompositionType.P || compType == CompositionType.PTZ)
                items.AddRange(_deviceManager.DeviceType.GetItemMetadata<PressureItems>());

            if (compType == CompositionType.T || compType == CompositionType.PTZ)
                items.AddRange(_deviceManager.DeviceType.GetItemMetadata<TemperatureItems>());

            if (compType == CompositionType.PTZ)
                items.AddRange(_deviceManager.DeviceType.GetItemMetadata<SuperFactorItems>());

            return items;
        }

        private void SetupRxUI()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(() => _testViewModelService.AddOrUpdate(TestViewModel));

            DownloadCommand =
                ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(DownloadItems);

            PrintTestReport =
                ReactiveCommand.CreateFromTask(Complete);

            RunVolumeTest = ReactiveCommand.CreateFromTask(VolumeTestManager.RunAsync);
        }
    }

    public partial class TestManagerViewModel : ITestManagerViewModelFactory
    {
        public TestManagerViewModel(ILoggerFactory loggerFactory,
            IScreenManager screenManager,
            DeviceSessionManager deviceManager,
            VerificationViewModelService testViewModelService,
            DialogServiceManager dialogService,
            IVolumeTestManagerFactory volumeTestManagerFactory)
        {
            _logger = loggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;

            _screenManager = screenManager;
            _deviceManager = deviceManager;
            _testViewModelService = testViewModelService;
            _dialogService = dialogService;
            _volumeTestManagerFactory = volumeTestManagerFactory;
        }

        public async Task<TestManagerViewModel> StartNew(DeviceType deviceType, string commPortName, int baudRate, string tachPortName)
        {
            await _deviceManager.StartSession(deviceType, commPortName, baudRate, null);
            TestViewModel = _testViewModelService.NewTest(_deviceManager.Device);
            VolumeTestManager = _volumeTestManagerFactory.CreateInstance(_deviceManager, TestViewModel.VolumeTest, tachPortName);
            SetupRxUI();
            return this;
        }
    }
}