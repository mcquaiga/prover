using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Services;
using Application.ViewModels;
using Application.ViewModels.Corrections;
using Client.Wpf.Screens.Dialogs;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Shared;

namespace Client.Wpf.Communications
{
    public class VerificationTestManager : ReactiveObject
    {
        private readonly IConfiguration _config;
        private readonly DeviceSessionManager _deviceManager;
        private readonly DialogServiceManager _dialogService;
        private readonly ILogger<VerificationTestManager> _logger;
        private readonly EvcVerificationTestService _testService;
        private readonly VerificationViewModelService _testViewModelService;

        public VerificationTestManager(ILogger<VerificationTestManager> logger,
            IConfiguration config,
            DeviceSessionManager deviceManager,
            VerificationViewModelService testViewModelService,
            EvcVerificationTestService testService,
            DialogServiceManager dialogService)
        {
            _logger = logger;
            _config = config;
            _deviceManager = deviceManager;
            _testViewModelService = testViewModelService;
            _testService = testService;
            _dialogService = dialogService;
        }

        public EvcVerificationViewModel TestViewModel { get; set; }

        public async Task Complete()
        {
            //await SaveCurrentState();
        }

        public async Task SaveCurrentState()
        {
            var evc = _testViewModelService.CreateVerificationTestFromViewModel(TestViewModel);
            await _testService.AddOrUpdateVerificationTest(evc);
        }

        public async Task StartNew(DeviceType deviceType, string commPortName, int baudRate, string tachPortName)
        {
            await _deviceManager.StartSession(deviceType, commPortName, baudRate, this);

            TestViewModel = _testViewModelService.NewTest(_deviceManager.Instance);
        }

        public async Task DownloadItems(VerificationTestPointViewModel test)
        {
            var toDownload = GetItemsToDownload(TestViewModel.CompositionType);

            var values = await _deviceManager.DownloadCorrectionItems(toDownload);

            test.UpdateItemValues(values);

            foreach (var correction in test.TestsCollection)
            {
                var testType = correction.GetType();
                if (testType.BaseType != null && testType.BaseType.IsGenericType &&
                    testType.BaseType.GetGenericTypeDefinition() == typeof(CorrectionTestViewModel<>))
                {
                    var itemType = testType.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));
                    itemType?.SetValue(correction,
                        _deviceManager.DeviceType.GetGroupValues(values, itemType.PropertyType));
                }
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
    }
}