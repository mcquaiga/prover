using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Screens.Dialogs;
using Client.Desktop.Wpf.ViewModels.Devices;
using Client.Desktop.Wpf.Views.Devices;
using Client.Desktop.Wpf.Views.Verifications.Dialogs;
using Devices.Communications.Status;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Interactions
{
    public static class DeviceInteractions
    {
        public static Interaction<IObservable<StatusMessage>, CancellationToken> Connecting { get; } =
            new Interaction<IObservable<StatusMessage>, CancellationToken>();

        public static Interaction<DeviceSessionManager, CancellationToken> Disconnecting { get; } =
            new Interaction<DeviceSessionManager, CancellationToken>();

        public static Interaction<DeviceSessionManager, CancellationToken> DownloadingItems { get; } =
            new Interaction<DeviceSessionManager, CancellationToken>();

        public static Interaction<ILiveReadHandler, CancellationToken> LiveReading { get; } =
            new Interaction<ILiveReadHandler, CancellationToken>();

        public static Interaction<DeviceSessionManager, Unit> Unlinked { get; } =
            new Interaction<DeviceSessionManager, Unit>();

        public static Interaction<VolumeTestManager, CancellationToken> StartVolumeTest { get; } =
            new Interaction<VolumeTestManager, CancellationToken>(); 
        public static Interaction<VolumeTestManager, CancellationToken> CompleteVolumeTest { get; } =
            new Interaction<VolumeTestManager, CancellationToken>();
        
    }

    public class DeviceSessionDialogManager : DialogViewModel
    {
        private readonly ILogger<DeviceSessionDialogManager> _logger;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CompositeDisposable _cleanup;
        private SessionDialogView _dialogView;
        private SessionStatusDialogView _sessionStatusView;

        public DeviceSessionDialogManager(ILogger<DeviceSessionDialogManager> logger,
            DialogServiceManager dialogManager)
        {
            _logger = logger ?? NullLogger<DeviceSessionDialogManager>.Instance;

            RegisterDeviceInteractions(dialogManager);

            RequestCancellation = ReactiveCommand.CreateFromTask(async () =>
            {
                _logger.LogDebug("Cancellation Requested.");
                _cancellationTokenSource?.Cancel();
                await dialogManager.CloseDialog.Execute();
            });
        }

        public ReactiveCommand<Unit, Unit> RequestCancellation { get; set; }
        [Reactive] public SessionStatusDialogViewModel SessionStatusUpdates { get; protected set; }
        [Reactive] public IViewFor SessionDialogContent { get; protected set; }

        private void RegisterDeviceInteractions(DialogServiceManager dialogManager)
        {
            DeviceInteractions.Connecting.RegisterHandler(async context =>
            {
                _dialogView = new SessionDialogView {ViewModel = this};

                _cancellationTokenSource = new CancellationTokenSource();
                SessionStatusUpdates = new SessionStatusDialogViewModel(context.Input, _cancellationTokenSource)
                {
                    StatusText = "Connecting ... "
                };

                _sessionStatusView = new SessionStatusDialogView {ViewModel = SessionStatusUpdates};
                SessionDialogContent = _sessionStatusView;

                await dialogManager.ShowDialogView.Execute(_dialogView);
                context.SetOutput(_cancellationTokenSource.Token);
            });

            DeviceInteractions.Disconnecting.RegisterHandler(async context =>
            {
                SessionDialogContent = _sessionStatusView;
                SessionStatusUpdates.StatusText = "Disconnecting ...";

                context.SetOutput(_cancellationTokenSource.Token);
            });

            DeviceInteractions.DownloadingItems.RegisterHandler(async context =>
            {
                SessionDialogContent = _sessionStatusView;
                SessionStatusUpdates.StatusText = "Downloading items ...";

                context.SetOutput(_cancellationTokenSource.Token);
            });

            DeviceInteractions.LiveReading.RegisterHandler(async context =>
            {
                SessionDialogContent = new LiveReadDialogView {ViewModel = (LiveReadCoordinator) context.Input};

                context.SetOutput(_cancellationTokenSource.Token);
            });

            DeviceInteractions.Unlinked.RegisterHandler(async context =>
            {
                await dialogManager.CloseDialog.Execute();
                context.SetOutput(Unit.Default);
            });

            DeviceInteractions.StartVolumeTest.RegisterHandler(async context =>
            {
                SessionDialogContent = new VolumeTestDialogView {ViewModel = context.Input};
               
                await dialogManager.ShowDialogView.Execute(_dialogView);
                context.SetOutput(_cancellationTokenSource.Token);
            }); 
            DeviceInteractions.CompleteVolumeTest.RegisterHandler(async context =>
            {
                //SessionDialogContent = new VolumeTestDialogView {ViewModel = context.Input};
                await dialogManager.CloseDialog.Execute();
                context.SetOutput(_cancellationTokenSource.Token);
            });
        }
    }
}