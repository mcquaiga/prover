using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Extensions;
using Prover.Application.Hardware;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels.Volume;
using Prover.Shared.Interfaces;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Application.Verifications.Volume {
	public abstract class AutomatedVolumeTestRunnerBase : IVolumeTestManager, IDisposable {
		private const int WaitTimeForResidualPulsesSeconds = 5;
		protected readonly CompositeDisposable Cleanup = new CompositeDisposable();
		protected ILogger Logger;

		protected IOutputChannel MotorControl;
		protected IAppliedInputVolume TachometerService;

		internal AutomatedVolumeTestRunnerBase(ILogger<RotaryVolumeTestRunner> logger,
			IDeviceSessionManager deviceManager,
			IAppliedInputVolume tachometerService,
			PulseOutputsListenerService pulseListenerService,
			IOutputChannel motorControl,
			VolumeViewModelBase volumeTest) {
			Logger = logger ?? NullLogger<RotaryVolumeTestRunner>.Instance;
			MotorControl = motorControl ?? throw new ArgumentNullException(nameof(motorControl));

			DeviceManager = deviceManager;

			TachometerService = tachometerService;
			PulseListenerService = pulseListenerService;
			VolumeTest = volumeTest;

			StartTest = ReactiveCommand.CreateFromTask(SyncVolume, outputScheduler: RxApp.MainThreadScheduler).DisposeWith(Cleanup);

			InitiateTestCompletion = ReactiveCommand.Create(() => {
				Logger.LogDebug("Stopping test.");
				MotorControl.SignalStop();

				Logger.LogDebug(
					$"Listening for residual pulses. Timeout period = {WaitTimeForResidualPulsesSeconds} seconds.");

				PulseListenerService.PulseCountUpdates
					.LogDebug(x => $"Waiting for residual pulses...")
					.Timeout(TimeSpan.FromMilliseconds(WaitTimeForResidualPulsesSeconds))
					.Throttle(TimeSpan.FromSeconds(WaitTimeForResidualPulsesSeconds))
					.LogDebug(x =>
						$"No new pulses received in {WaitTimeForResidualPulsesSeconds} seconds. Initiating test completion...")
					.Select(_ => Unit.Default)
					.ObserveOn(RxApp.MainThreadScheduler)
					.OnErrorResumeNext(Observable.Empty<Unit>())
					.InvokeCommand(FinishTest)
					.DisposeWith(Cleanup);

			}, outputScheduler: RxApp.MainThreadScheduler);

			FinishTest = ReactiveCommand.CreateFromTask(CompleteVolumeVerification, outputScheduler: RxApp.MainThreadScheduler)
				.DisposeWith(Cleanup);

			StartTest.DisposeWith(Cleanup);
			FinishTest.DisposeWith(Cleanup);
			InitiateTestCompletion.DisposeWith(Cleanup);
			TachometerService.DisposeWith(Cleanup);

		}

		public ReactiveCommand<Unit, Unit> StartTest { get; protected set; }
		public ReactiveCommand<Unit, Unit> FinishTest { get; protected set; }

		public IDeviceSessionManager DeviceManager { get; }
		public PulseOutputsListenerService PulseListenerService { get; }
		public VolumeViewModelBase VolumeTest { get; }
		public ReactiveCommand<Unit, Unit> InitiateTestCompletion { get; protected set; }

		public virtual int TargetUncorrectedPulses { get; } = 10;

		public void Dispose() {
			Cleanup?.Dispose();
		}

		public abstract Task PublishCompleteInteraction();

		public abstract Task<CancellationToken> PublishStartInteraction();

		protected abstract Task ExecuteTestAsync();

		protected virtual void UpdatePulseOutputTestCounts() {
			foreach (var test in VolumeTest.AllTests().OfType<VolumeTestRunViewModelBase>()) {
				var pulseTest = test.PulseOutputTest;

				var pulser = PulseListenerService.PulseChannels.FirstOrDefault(p => p.Channel == pulseTest.Items.Name);

				if (pulser != null)
					pulseTest.ActualValue = pulser.PulseCount;
			}
		}

		public virtual async Task CompleteVolumeVerification() {
			Logger.LogDebug("Completing test...");

			PulseListenerService.Stop();
			UpdatePulseOutputTestCounts();
			VolumeTest.Uncorrected.AppliedInput = await TachometerService.GetAppliedInput();

			await PublishCompleteInteraction();

			var endValues = await DeviceManager.GetItemValues();
			VolumeTest.Uncorrected.EndValues = DeviceManager.Device.DeviceType.GetGroup<VolumeItems>(endValues);
			VolumeTest.Corrected.EndValues = DeviceManager.Device.DeviceType.GetGroup<VolumeItems>(endValues);
			VolumeTest.EndValues = DeviceManager.Device.DeviceType.GetGroup<VolumeItems>(endValues);
			await DeviceManager.Disconnect();
		}

		public virtual async Task BeginVolumeVerification() {

			Logger.LogInformation(
				$"Starting {DeviceManager.Device.DriveType} volume test for {DeviceManager.Device.DeviceType}.");


			await TachometerService.ResetAppliedInput();

			var startValues = await DeviceManager.GetItemValues();
			VolumeTest.Uncorrected.StartValues = DeviceManager.Device.DeviceType.GetGroup<VolumeItems>(startValues);
			VolumeTest.Corrected.StartValues = DeviceManager.Device.DeviceType.GetGroup<VolumeItems>(startValues);
			VolumeTest.StartValues = DeviceManager.Device.DeviceType.GetGroup<VolumeItems>(startValues);
			await DeviceManager.Disconnect();

			var cancelToken = await PublishStartInteraction();
			cancelToken.Register(CancelTest);

			await ExecuteTestAsync();
		}

		protected virtual async Task SyncVolume() {
			Logger.LogDebug("Syncing volume...");
			MotorControl.SignalStart();

			PulseListenerService.StartListening()
				.Where(channel => channel.Items.ChannelType == Shared.PulseOutputType.UncVol && channel.PulseCount == 1)
				.LogDebug(x => "Sync complete!")
				.Do(_ => MotorControl.SignalStop())
				.InvokeMethod("BeginVolumeVerification");
		}


		protected virtual void CancelTest() {
			Logger.LogWarning("Cancelling test...");
			MotorControl.SignalStop();
			PulseListenerService.Stop();
		}
	}
}