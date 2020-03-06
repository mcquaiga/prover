using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Interactions;
using Client.Desktop.Wpf.Screens.Dialogs;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Prover.Application.Extensions;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using Prover.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public partial class TestManager : RoutableViewModelBase, IRoutableViewModel, IDisposable, IDialogViewModel
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly IDeviceSessionManager _deviceManager;
        private readonly DialogServiceManager _dialogService;
        private readonly ILogger _logger;
        private readonly IScreenManager _screenManager;
        private readonly VerificationViewModelService _testViewModelService;
        private readonly IVolumeTestManagerFactory _volumeTestManagerFactory;

        [Reactive] public EvcVerificationViewModel TestViewModel { get; protected set; }

        [Reactive] public VolumeTestManager VolumeTestManager { get; protected set; }

        public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
        public ReactiveCommand<VerificationTestPointViewModel, Unit> DownloadCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
        public ReactiveCommand<Unit, Unit> CompleteTest { get; protected set; }

        public ReactiveCommand<Unit, Unit> RunVolumeTest { get; protected set; }

        public override string UrlPathSegment => "/VerificationTests/Details";
        public override IScreen HostScreen => _screenManager;

        public ReactiveCommand<Unit, bool> ShowCommand { get; set; }
        public ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        public bool IsDialogOpen { get; }

        public async Task Complete()
        {
            //await SaveCurrentState();
        }

        public async Task RunCorrectionTests(VerificationTestPointViewModel test)
        {
            var toDownload = GetItemsToDownload(_deviceManager.Device.Composition());

            //Wait for temperature/pressure to stabilize
            await StabilizeLiveReadings(test);

            //var values = await _deviceManager.DownloadCorrectionItems(toDownload);

            //test.UpdateItemValues(values);

            //foreach (var correction in test.GetCorrectionTests())
            //{
            //    var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));
            //    itemType?.SetValue(correction,
            //        _deviceManager.DeviceType.GetGroupValues(values, itemType.PropertyType));
            //}
        }

        private async Task StabilizeLiveReadings(VerificationTestPointViewModel test)
        {
            var liveItems = new Dictionary<ItemMetadata, AveragedReadingStabilizer>();
            var toDownload = GetItemsToDownload(_deviceManager.Device.Composition());
            var items = new List<ItemMetadata>();

            if (test.Pressure != null)
            {
                var pressureItem = toDownload.First(i => i.IsLiveReadPressure == true);
                liveItems.Add(pressureItem, new AveragedReadingStabilizer(test.Pressure.GetTotalGauge()));
                items.Add(pressureItem);
            }
            
            if (test.Temperature != null)
            {
                var tempItem = toDownload.First(i => i.IsLiveReadTemperature == true);
                liveItems.Add(tempItem, new AveragedReadingStabilizer(test.Temperature.Gauge));
                items.Add(tempItem);
            }

            await _deviceManager.LiveReadItem(items, new CancellationTokenSource());
        }

        protected override void Disposing()
        {
            _logger.LogDebug($"Disposing instance created at {_dateCreated}.");
            _cleanup?.Dispose();
        }

        protected void SetupRxUi()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var success = await _testViewModelService.AddOrUpdate(TestViewModel);
                if (success) await NotificationInteractions.SnackBarMessage.Handle("Saved Successfully!");
                return success;
            });
            SaveCommand.ThrownExceptions.LogErrors("Error saving test.").Subscribe();

            DownloadCommand = ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(RunCorrectionTests);
            DownloadCommand.ThrownExceptions.LogErrors("Error downloading items from instrument.").Subscribe();

            PrintTestReport = ReactiveCommand.CreateFromObservable(() =>
                MessageInteractions.ShowMessage.Handle("Verifications Report feature not yet implemented."));

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
                //DownloadCommand
                //    .InvokeCommand(SaveCommand);

                //this.WhenAnyValue(x => x.TestViewModel);
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

        private async Task Reset()
        {
            await _deviceManager.EndSession();
        }

        private void RunNavigatingAwayActions()
        {
            // Ask if they want to save
        }
    }
}