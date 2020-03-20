using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Interactions;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume;
using Prover.Shared;
using Prover.Shared.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.Communications
{
    public class VolumeTestManager : ViewModelBase
    {
        private const int WaitTimeForResidualPulsesSeconds = 5;
        private readonly ILogger _logger;
        private readonly IOutputChannel _motorControl;
        protected readonly ITachometerService TachometerService;

        internal VolumeTestManager(ILogger<VolumeTestManager> logger,
            IDeviceSessionManager deviceManager,
            ITachometerService tachometerService,
            PulseOutputsListenerService pulseListenerService,
            VolumeViewModelBase volumeTest,
            IOutputChannel motorControl)
        {
            _logger = logger ?? NullLogger<VolumeTestManager>.Instance;
            _motorControl = motorControl ?? throw new ArgumentNullException(nameof(motorControl));

            TachometerService = tachometerService;
            PulseListenerService = pulseListenerService;
            VolumeTest = volumeTest;
            DeviceManager = deviceManager;

            StartTest = ReactiveCommand.CreateFromTask(async () =>
            {
                _logger.LogInformation("Starting volume test.");
                await PreTestActions();
                await ExecuteTestAsync();
            }, outputScheduler: RxApp.MainThreadScheduler);

            InitiateTestCompletion = ReactiveCommand.Create(() =>
            {
                _logger.LogDebug("Stopping test.");
                _motorControl.SignalStop();

                _logger.LogDebug(
                    $"Waiting for residual pulses. Timeout period = {WaitTimeForResidualPulsesSeconds} seconds.");

                PulseListenerService.PulseCountUpdates
                    .Throttle(TimeSpan.FromSeconds(WaitTimeForResidualPulsesSeconds))
                    .LogDebug(x => "Timed out listening for residual pulses.")
                    .Select(_ => Unit.Default)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .InvokeCommand(CompleteTest)
                    .DisposeWith(Cleanup);
            }, outputScheduler: RxApp.MainThreadScheduler);

            CompleteTest = ReactiveCommand.CreateFromTask(async () =>
            {
                _logger.LogInformation("Completing test.");

                PulseListenerService.Stop();
                UpdatePulseOutputTestCounts();

                await DeviceInteractions.CompleteVolumeTest.Handle(this);
                await PostTestActions();
            }, outputScheduler: RxApp.MainThreadScheduler);

            StartTest.DisposeWith(Cleanup);
            CompleteTest.DisposeWith(Cleanup);
            InitiateTestCompletion.DisposeWith(Cleanup);
        }

        public PulseOutputsListenerService PulseListenerService { get; }
        public ReactiveCommand<Unit, Unit> StartTest { get; protected set; }
        public ReactiveCommand<Unit, Unit> CompleteTest { get; protected set; }
        public ReactiveCommand<Unit, Unit> InitiateTestCompletion { get; protected set; }

        public VolumeViewModelBase VolumeTest { get; }
        public IDeviceSessionManager DeviceManager { get; }

        public int TargetUncorrectedPulses => VolumeTest.AllTests().OfType<UncorrectedVolumeTestViewModel>().First().DriveType.MaxUncorrectedPulses();

        protected virtual async Task ExecuteTestAsync()
        {
            _logger.LogDebug("Executing automated volume test.");

            var cancelToken = await DeviceInteractions.StartVolumeTest.Handle(this);
            cancelToken.Register(() =>
            {
                _logger.LogWarning("Cancelling test...");
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

        protected virtual async Task PostTestActions()
        {
            var endValues = await DeviceManager.GetItemValues();
            VolumeTest.EndValues = DeviceManager.Device.DeviceType.GetGroupValues<VolumeItems>(endValues);
        }

        protected virtual async Task PreTestActions()
        {
            var startValues = await DeviceManager.GetItemValues();
            VolumeTest.StartValues = DeviceManager.Device.DeviceType.GetGroupValues<VolumeItems>(startValues);
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