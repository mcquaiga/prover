using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Screens.Dialogs;
using Client.Desktop.Wpf.ViewModels.Verifications.Dialogs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Services;
using Prover.Application.ViewModels.Volume;
using Prover.Shared;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Communications
{
    public interface IVolumeTestManagerFactory
    {
        //VolumeTestManager CreateInstance(DeviceInstance device, VolumeViewModelBase volumeTest);
        VolumeTestManager CreateInstance(DeviceSessionManager deviceManager, VolumeViewModelBase volumeTest,
            string tachPortName = null);
    }


    public class VolumeTestManager : ReactiveObject
    {
        private readonly DialogServiceManager _dialogService;
        private readonly ILogger _logger;
        private readonly IOutputChannel _motorControl;
        protected readonly PulseInputsListenerService PulseListenerService;
        protected readonly ITachometerService TachometerService;
        private CancellationTokenSource _cancellationToken;

        protected IObservable<VolumeTestStatusMessage> TestStatusObservable;

        internal VolumeTestManager(ILogger logger,
            DialogServiceManager dialogService,
            DeviceSessionManager deviceManager,
            ITachometerService tachometerService,
            PulseInputsListenerService pulseListenerService,
            VolumeViewModelBase volumeTest,
            IOutputChannel motorControl)
        {
            _logger = logger ?? NullLogger.Instance;
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(DialogServiceManager));
            _motorControl = motorControl ?? throw new ArgumentNullException(nameof(motorControl));

            TachometerService = tachometerService;
            PulseListenerService = pulseListenerService;
            VolumeTest = volumeTest;
            DeviceManager = deviceManager;

            StartVolumeTest = ReactiveCommand.CreateFromTask(RunAsync);

            //PulseListenerService.PulseListener.Connect();
        }

        [Reactive] public ReactiveCommand<Unit, Unit> StartVolumeTest { get; protected set; }

        public VolumeViewModelBase VolumeTest { get; protected set; }
        public DeviceSessionManager DeviceManager { get; protected set; }

        public async Task RunAsync()
        {
            _cancellationToken = new CancellationTokenSource();

            var startValues = await DeviceManager.GetItemValues();
            VolumeTest.UpdateStartValues(startValues);

            DisplayDialog();

            _motorControl.SignalStart();

            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        protected async Task CompleteAsync()
        {
            _dialogService.CloseDialog.Execute();

            var endValues = await DeviceManager.GetItemValues();
            VolumeTest.UpdateEndValues(endValues);
        }

        protected void DisplayDialog()
        {
            var dialog = new VolumeTestDialogViewModel(TestStatusObservable, _cancellationToken);
            _dialogService.ShowDialog.Execute(dialog);
        }
    }

    public class VolumeTestStatusMessage
    {
        public VolumeTestStatusMessage(PulseOutputChannel channel)
        {
        }
    }
}