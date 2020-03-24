using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume;
using Prover.Shared.Interfaces;
using ReactiveUI;

namespace Prover.Application.VerificationManager.Volume
{
    public abstract class AutomatedVolumeTestManagerBase : IVolumeTestManager, IDisposable
    {
        private const int WaitTimeForResidualPulsesSeconds = 5;
        protected readonly CompositeDisposable Cleanup = new CompositeDisposable();
        protected ILogger Logger;

        protected IOutputChannel MotorControl;
        protected ITachometerService TachometerService;

        internal AutomatedVolumeTestManagerBase(ILogger<RotaryVolumeManager> logger,
            IDeviceSessionManager deviceManager,
            ITachometerService tachometerService,
            PulseOutputsListenerService pulseListenerService,
            IOutputChannel motorControl,
            VolumeViewModelBase volumeTest)
        {
            Logger = logger ?? NullLogger<RotaryVolumeManager>.Instance;
            MotorControl = motorControl ?? throw new ArgumentNullException(nameof(motorControl));

            DeviceManager = deviceManager;

            TachometerService = tachometerService;
            PulseListenerService = pulseListenerService;
            VolumeTest = volumeTest;

            StartTest = ReactiveCommand.CreateFromTask(async () => { await RunStartActions(); },
                outputScheduler: RxApp.MainThreadScheduler);

            InitiateTestCompletion = ReactiveCommand.Create(() =>
            {
                Logger.LogDebug("Stopping test.");
                MotorControl.SignalStop();

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

            FinishTest = ReactiveCommand.CreateFromTask(async () => { await RunFinishActions(); },
                outputScheduler: RxApp.MainThreadScheduler);

            StartTest.DisposeWith(Cleanup);
            FinishTest.DisposeWith(Cleanup);
            InitiateTestCompletion.DisposeWith(Cleanup);
        }

        public ReactiveCommand<Unit, Unit> StartTest { get; protected set; }
        public ReactiveCommand<Unit, Unit> FinishTest { get; protected set; }

        public IDeviceSessionManager DeviceManager { get; }
        public PulseOutputsListenerService PulseListenerService { get; }
        public VolumeViewModelBase VolumeTest { get; }
        public ReactiveCommand<Unit, Unit> InitiateTestCompletion { get; protected set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public abstract Task PublishCompleteInteraction();

        public abstract Task<CancellationToken> PublishStartInteraction();

        public virtual async Task RunFinishActions()
        {
            Logger.LogDebug("Completing test...");

            PulseListenerService.Stop();
            UpdatePulseOutputTestCounts();

            await PublishCompleteInteraction();

            var endValues = await DeviceManager.GetItemValues();
            VolumeTest.EndValues = DeviceManager.Device.DeviceType.GetGroupValues<VolumeItems>(endValues);
        }

        public virtual async Task RunStartActions()
        {
            Logger.LogInformation(
                $"Starting {VolumeTest.DriveType.InputType} test for {DeviceManager.Device.DeviceType}.");
            var startValues = await DeviceManager.GetItemValues();
            VolumeTest.StartValues = DeviceManager.Device.DeviceType.GetGroupValues<VolumeItems>(startValues);

            var cancelToken = await PublishStartInteraction();
            cancelToken.Register(() =>
            {
                Logger.LogWarning("Cancelling test...");
                MotorControl.SignalStop();
                PulseListenerService.Stop();
            });

            await ExecuteTestAsync();
        }

        protected abstract Task ExecuteTestAsync();

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