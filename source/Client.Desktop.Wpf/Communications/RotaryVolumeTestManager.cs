using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume;
using Prover.Shared;
using Prover.Shared.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.Communications
{
    public interface IVolumeTestManager
    {
        public Task RunFinishActions();
        public Task RunStartActions();
    }

    public abstract class VolumeTestManagerBase : ViewModelBase, IVolumeTestManager
    {
        protected ILogger Logger;

        protected VolumeTestManagerBase(ILogger logger, IDeviceSessionManager deviceManager, VolumeViewModelBase volumeTest)
        {
            DeviceManager = deviceManager;
            VolumeTest = volumeTest;
            Logger = logger;

            StartTest = ReactiveCommand.CreateFromTask(async () => { await RunStartActions(); },
                outputScheduler: RxApp.MainThreadScheduler);

            FinishTest = ReactiveCommand.CreateFromTask(async () => { await RunFinishActions(); },
                outputScheduler: RxApp.MainThreadScheduler);

            StartTest.DisposeWith(Cleanup);
            FinishTest.DisposeWith(Cleanup);
        }

        public ReactiveCommand<Unit, Unit> StartTest { get; protected set; }
        public ReactiveCommand<Unit, Unit> FinishTest { get; protected set; }
        public VolumeViewModelBase VolumeTest { get; }
        public IDeviceSessionManager DeviceManager { get; }
        public virtual async Task RunFinishActions() { }
        public virtual async Task RunStartActions() { }
    }

    public class RotaryVolumeTestManager : VolumeTestManagerBase
    {
        private const int WaitTimeForResidualPulsesSeconds = 5;
        private readonly IOutputChannel _motorControl;
        protected readonly ITachometerService TachometerService;

        internal RotaryVolumeTestManager(ILogger<RotaryVolumeTestManager> logger,
            IDeviceSessionManager deviceManager,
            ITachometerService tachometerService,
            PulseOutputsListenerService pulseListenerService,
            VolumeViewModelBase volumeTest,
            IOutputChannel motorControl) : base(logger, deviceManager, volumeTest)
        {
            Logger = logger ?? NullLogger<RotaryVolumeTestManager>.Instance;
            _motorControl = motorControl ?? throw new ArgumentNullException(nameof(motorControl));

            TachometerService = tachometerService;
            PulseListenerService = pulseListenerService;

            InitiateTestCompletion = ReactiveCommand.Create(() =>
            {
                Logger.LogDebug("Stopping test.");
                _motorControl.SignalStop();

                Logger.LogDebug(
                    $"Listening for residual pulses. Timeout period = {WaitTimeForResidualPulsesSeconds} seconds.");
                PulseListenerService.PulseCountUpdates
                    .Throttle(TimeSpan.FromSeconds(WaitTimeForResidualPulsesSeconds))
                    .LogDebug(x =>
                        $"No new pulses received in {WaitTimeForResidualPulsesSeconds} seconds. Initiating test completion...")
                    .Select(_ => Unit.Default)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .InvokeCommand(FinishTest)
                    .DisposeWith(Cleanup);

            }, outputScheduler: RxApp.MainThreadScheduler);

            InitiateTestCompletion.DisposeWith(Cleanup);
        }

        public PulseOutputsListenerService PulseListenerService { get; }
        public ReactiveCommand<Unit, Unit> InitiateTestCompletion { get; protected set; }

        public int TargetUncorrectedPulses => VolumeTest.AllTests().OfType<UncorrectedVolumeTestViewModel>().First()
            .DriveType.MaxUncorrectedPulses();

        protected virtual async Task ExecuteTestAsync()
        {
            Logger.LogDebug("Executing automated volume test.");

            var cancelToken = await DeviceInteractions.StartVolumeTest.Handle(this);
            cancelToken.Register(() =>
            {
                Logger.LogWarning("Cancelling test...");
                _motorControl.SignalStop();
                PulseListenerService.Stop();
            });

            PulseListenerService.StartListening()
                .Where(p =>
                    p.Items.Units == PulseOutputUnitType.UncVol && p.PulseCount == TargetUncorrectedPulses)
                .Select(_ => Unit.Default)
                .InvokeCommand(InitiateTestCompletion)
                .DisposeWith(Cleanup);

            _motorControl.SignalStart();
        }

        public override async Task RunFinishActions()
        {
            Logger.LogDebug("Completing test...");

            PulseListenerService.Stop();
            UpdatePulseOutputTestCounts();

            await DeviceInteractions.CompleteVolumeTest.Handle(this);

            var endValues = await DeviceManager.GetItemValues();
            VolumeTest.EndValues = DeviceManager.Device.DeviceType.GetGroupValues<VolumeItems>(endValues);
        }

        public override async Task RunStartActions()
        {
            Logger.LogInformation(
                $"Starting {VolumeTest.DriveType.InputType} test for {DeviceManager.Device.DeviceType}.");
            var startValues = await DeviceManager.GetItemValues();
            VolumeTest.StartValues = DeviceManager.Device.DeviceType.GetGroupValues<VolumeItems>(startValues);

            await ExecuteTestAsync();
        }

        protected virtual void UpdatePulseOutputTestCounts()
        {
            foreach (var test in VolumeTest.AllTests().OfType<VolumeTestRunViewModelBase>())
            {
                var pulseTest = test.PulseOutputTest;

                var pulser = PulseListenerService.PulseChannels.FirstOrDefault(p => p.Channel == pulseTest.Items.Name);

                if (pulser != null) pulseTest.ExpectedValue = pulser.PulseCount;
            }
        }
    }
}