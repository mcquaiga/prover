using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public DialogServiceManager DialogManager { get; }
        private readonly ILogger<DeviceSessionDialogManager> _logger;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CompositeDisposable _cleanup;
        private SessionDialogView _dialogView;
        private SessionStatusDialogView _sessionStatusView;

        public DeviceSessionDialogManager(ILogger<DeviceSessionDialogManager> logger,
            DialogServiceManager dialogManager)
        {
            DialogManager = dialogManager;
            _logger = logger ?? NullLogger<DeviceSessionDialogManager>.Instance;

            RegisterDeviceInteractions();

            RequestCancellation = ReactiveCommand.CreateFromTask(async () =>
            {
                _logger.LogDebug("Cancellation Requested.");
                _cancellationTokenSource?.Cancel();
                await dialogManager.CloseDialog.Execute();
            });
        }

        public ReactiveCommand<Unit, Unit> RequestCancellation { get; protected set; }

        protected SessionStatusDialogViewModel SessionStatusUpdates;
        [Reactive] public IViewFor SessionDialogContent { get; protected set; }

        public void RegisterDeviceInteractions()
        {
            DeviceInteractions.Connecting.RegisterHandler(async context =>
            {
                _cancellationTokenSource = new CancellationTokenSource();

                SetSessionStatusDialog("Connecting", context.Input);

                await DialogManager.ShowDialogView.Execute(_dialogView);
                context.SetOutput(_cancellationTokenSource.Token);
            });

            DeviceInteractions.Disconnecting.RegisterHandler(async context =>
            {
                SetSessionStatusDialog("Disconnecting");

                context.SetOutput(_cancellationTokenSource.Token);
            });

            DeviceInteractions.DownloadingItems.RegisterHandler(async context =>
            {
                SetSessionStatusDialog("Downloading items");

                context.SetOutput(_cancellationTokenSource.Token);
            });

            DeviceInteractions.LiveReading.RegisterHandler(async context =>
            {
                SessionDialogContent = new LiveReadDialogView {ViewModel = (LiveReadCoordinator) context.Input};

                context.SetOutput(_cancellationTokenSource.Token);
            });

            DeviceInteractions.Unlinked.RegisterHandler(async context =>
            {
                await DialogManager.CloseDialog.Execute();
                context.SetOutput(Unit.Default);
            });

            DeviceInteractions.StartVolumeTest.RegisterHandler(async context =>
            {
                SessionDialogContent = new VolumeTestDialogView {ViewModel = context.Input};

                await DialogManager.ShowDialogView.Execute(_dialogView);
                context.SetOutput(_cancellationTokenSource.Token);
            });

            DeviceInteractions.CompleteVolumeTest.RegisterHandler(async context =>
            {
                await DialogManager.CloseDialog.Execute();
                context.SetOutput(_cancellationTokenSource.Token);
            });
        }

        private void SetSessionStatusDialog(string message, IObservable<StatusMessage> statusMessageObservable = null)
        {
            if (_dialogView == null) _dialogView = new SessionDialogView {ViewModel = this};
            if (_sessionStatusView == null)
            {
                SessionStatusUpdates =
                    new SessionStatusDialogViewModel(statusMessageObservable, _cancellationTokenSource)
                    {
                        StatusText = message
                    };

                _sessionStatusView = new SessionStatusDialogView {ViewModel = SessionStatusUpdates};
            }
            
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                SessionStatusUpdates.StatusText = message;
                SessionDialogContent = _sessionStatusView;
            });
        }
    }
}