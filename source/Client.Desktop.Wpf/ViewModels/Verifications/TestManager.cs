using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Interactions;
using Client.Desktop.Wpf.Screens.Dialogs;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public partial class TestManager : RoutableViewModelBase, IRoutableViewModel, IDisposable
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly IDeviceSessionManager _deviceManager;
        private readonly ILogger _logger;
        private readonly IScreenManager _screenManager;
        private readonly VerificationViewModelService _testViewModelService;

        public EvcVerificationViewModel TestViewModel { get; protected set; }
        public VolumeTestManager VolumeTestManager { get; protected set; }

        public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
        public ReactiveCommand<VerificationTestPointViewModel, Unit> DownloadCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
        public ReactiveCommand<Unit, Unit> SubmitTest { get; protected set; }

        public override string UrlPathSegment => "/VerificationTests/Details";
        public override IScreen HostScreen => _screenManager;

        //public ReactiveCommand<Unit, bool> ShowCommand { get; set; }
        //public ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        //public bool IsDialogOpen { get; }

        public async Task RunCorrectionTests(VerificationTestPointViewModel test)
        {
            await LiveReadCoordinator.StartLiveReading(_deviceManager, test,
                async () =>
                {
                    var values = await _deviceManager.DownloadCorrectionItems();

                    test.UpdateItemValues(values);

                    foreach (var correction in test.GetCorrectionTests())
                    {
                        var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));
                        itemType?.SetValue(correction,
                            _deviceManager.Device.DeviceType.GetGroupValues(values, itemType.PropertyType));
                    }
                }, RxApp.MainThreadScheduler);
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

            SubmitTest = ReactiveCommand.CreateFromTask(async () =>
            {
                await SaveCommand.Execute();
                await _deviceManager.EndSession();
            });

            SetupAutoSave();

            SaveCommand.DisposeWith(_cleanup);
            DownloadCommand.DisposeWith(_cleanup);
            PrintTestReport.DisposeWith(_cleanup);

            void SetupAutoSave()
            {
                //DownloadCommand
                //    .InvokeCommand(SaveCommand);

                //this.WhenAnyValue(x => x.TestViewModel);
            }
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