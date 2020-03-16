using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Interactions;
using Client.Desktop.Wpf.Screens.Dialogs;
using Client.Desktop.Wpf.ViewModels.Verifications.Dialogs;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using DynamicData;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
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
        VolumeTestManager CreateInstance(IDeviceSessionManager deviceManager, VolumeViewModelBase volumeTest,
            string tachPortName = null);
    }

    public class VolumeTestManager : ViewModelBase
    {
        private readonly DialogServiceManager _dialogService;
        private readonly ILogger _logger;
        private readonly IOutputChannel _motorControl;
        protected readonly PulseInputsListenerService PulseListenerService;
        protected readonly ITachometerService TachometerService;
        private CancellationTokenSource _cancellationToken;

        protected IObservable<VolumeTestStatusMessage> TestStatusObservable;

        internal VolumeTestManager(ILogger<VolumeTestManager> logger,
            DialogServiceManager dialogService,
            IDeviceSessionManager deviceManager,
            ITachometerService tachometerService,
            PulseInputsListenerService pulseListenerService,
            VolumeViewModelBase volumeTest,
            IOutputChannel motorControl)
        {
            _logger = logger ?? NullLogger<VolumeTestManager>.Instance;
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(DialogServiceManager));
            _motorControl = motorControl ?? throw new ArgumentNullException(nameof(motorControl));

            TachometerService = tachometerService;
            PulseListenerService = pulseListenerService;
            VolumeTest = volumeTest;
            DeviceManager = deviceManager;

            StartVolumeTest = ReactiveCommand.CreateFromTask(RunAsync);
            StartVolumeTest.DisposeWith(Cleanup);
            //PulseListenerService.PulseListener.Connect();
        }

        [Reactive] public ReactiveCommand<Unit, Unit> StartVolumeTest { get; protected set; }

        public VolumeViewModelBase VolumeTest { get; protected set; }
        public IDeviceSessionManager DeviceManager { get; protected set; }

        public ReadOnlyObservableCollection<PulseChannel> PulseOutputCounters { get; protected set; }

        public async Task RunAsync()
        {
            _cancellationToken = new CancellationTokenSource();

            var startValues = await DeviceManager.GetItemValues();
            UpdateStartValues(startValues);

            var cancelToken =
                await DeviceInteractions.StartVolumeTest.Handle(this);

            PulseListenerService.StartListening().Connect()
                .Bind(out var pulseUpdates)
                .Subscribe()
                .DisposeWith(Cleanup);

            PulseListenerService.PulseCountUpdates.Connect().AsObservable()
            //    .LogDebug(set => $"Pulse {set.Updates}")
            //    .Subscribe()
            //    .DisposeWith(Cleanup);

            PulseOutputCounters = pulseUpdates;
            
            _motorControl.SignalStart();

            await Task.Delay(TimeSpan.FromSeconds(15));

            await CompleteAsync();
        }

        protected async Task CompleteAsync()
        {
            _motorControl.SignalStop();
            PulseListenerService.Dispose();

            var cancelToken =
                await DeviceInteractions.CompleteVolumeTest.Handle(this);

            var endValues = await DeviceManager.GetItemValues();
            UpdateEndValues(endValues);
        }

        //protected void DisplayDialog()
        //{
        //    DeviceInteractions.StartVolumeTest.Handle(this);

        //    var dialog = new VolumeTestDialogViewModel(TestStatusObservable, _cancellationToken);
        //    _dialogService.ShowDialog.Execute(dialog);
        //}

        public virtual void UpdateStartValues(IEnumerable<ItemValue> values)
        {
            VolumeTest.StartValues = DeviceManager.Device.DeviceType.GetGroupValues<VolumeItems>(values);
        }

        public virtual void UpdateEndValues(IEnumerable<ItemValue> values)
        {
            VolumeTest.EndValues = DeviceManager.Device.DeviceType.GetGroupValues<VolumeItems>(values);
        }


    }

    public class VolumeTestStatusMessage
    {
        public VolumeTestStatusMessage(PulseOutputChannel channel)
        {
        }
    }
}