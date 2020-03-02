using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Interactions;
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
    
    public partial class TestManagerViewModel : RoutableViewModelBase, IRoutableViewModel, IDisposable, IDialogViewModel
    {
        private readonly DeviceSessionManager _deviceManager;
        private readonly DialogServiceManager _dialogService;
        private readonly ILogger _logger;
        private readonly IScreenManager _screenManager;
        private readonly VerificationViewModelService _testViewModelService;
        private readonly IVolumeTestManagerFactory _volumeTestManagerFactory;
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();

        [Reactive] public EvcVerificationViewModel TestViewModel { get; protected set; }

        [Reactive] public VolumeTestManager VolumeTestManager { get; protected set; }

        public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
        public ReactiveCommand<VerificationTestPointViewModel, Unit> DownloadCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
        public ReactiveCommand<Unit, Unit> CompleteTest { get; protected set; }

        public ReactiveCommand<Unit, Unit> RunVolumeTest { get; protected set; }

        public override string UrlPathSegment => "/VerificationTests/Details";
        public override IScreen HostScreen => _screenManager;

        public async Task Complete()
        {
            //await SaveCurrentState();
        }

        public void Dispose()
        {
            _logger.LogDebug($"Disposing instance created at {_dateCreated}." );
            _cleanup?.Dispose();
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

        private void RunNavigatingAwayActions()
        {
            
            // Ask if they want to save
        }

        private void SetupRxUi()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var success = await _testViewModelService.AddOrUpdate(TestViewModel);
                if (success) await NotificationInteractions.SnackBarMessage.Handle("Saved Successfully!");
                return success;
            });
            SaveCommand.ThrownExceptions.LogErrors("Error saving test.").Subscribe();

            DownloadCommand = ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(DownloadItems);
            DownloadCommand.ThrownExceptions.LogErrors("Error downloading items from instrument.").Subscribe();

            PrintTestReport = ReactiveCommand.CreateFromObservable(() => MessageInteractions.ShowMessage.Handle("Verifications Report feature not yet implemented."));

            RunVolumeTest = ReactiveCommand.CreateFromTask(VolumeTestManager.RunAsync);

            CompleteTest = ReactiveCommand.CreateFromTask(async () =>
            {
                await SaveCommand.Execute();
                await _deviceManager.EndSession();
            });

            SetupAutoSave();

            SaveCommand.DisposeWith(_cleanup);
            DownloadCommand.DisposeWith(_cleanup);
            PrintTestReport.DisposeWith(_cleanup);
            RunVolumeTest.DisposeWith(_cleanup);
            
            void SetupAutoSave()
            {
                DownloadCommand
                    .InvokeCommand(SaveCommand);

                //this.WhenAnyValue(x => x.TestViewModel);

            }
        }

        async Task Reset()
        {
            await _deviceManager.EndSession();
        }

        public ReactiveCommand<Unit, bool> ShowCommand { get; set; }
        public ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        public bool IsDialogOpen { get; }
    }

    public partial class TestManagerViewModel : ITestManagerViewModelFactory
    {
        private readonly DateTime _dateCreated;

        public TestManagerViewModel(ILoggerFactory loggerFactory,
            IScreenManager screenManager,
            DeviceSessionManager deviceManager,
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

            this.WhenNavigatingFromObservable()
                .Subscribe(async _ =>
                {
                    _logger.LogDebug($"Navigating away from Cleaning up.");
                    await _deviceManager.EndSession();
                    RunNavigatingAwayActions();
                })
                .DisposeWith(_cleanup);
        }

        public async Task<TestManagerViewModel> StartNew(DeviceType deviceType, string commPortName, int baudRate,
            string tachPortName)
        {
            if (_deviceManager.SessionInProgress) await Reset();

            await _deviceManager.StartSession(deviceType, commPortName, baudRate, null);
            
            TestViewModel = _testViewModelService.NewTest(_deviceManager.Device);
            TestViewModel.DisposeWith(_cleanup);

            VolumeTestManager = _volumeTestManagerFactory.CreateInstance(_deviceManager, TestViewModel.VolumeTest, tachPortName);
            VolumeTestManager.DisposeWith(_cleanup);

            SetupRxUi();

            await _screenManager.ChangeView(this);
            
            return this;
        }
    }

}