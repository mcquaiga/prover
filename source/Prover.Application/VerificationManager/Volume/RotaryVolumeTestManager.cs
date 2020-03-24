using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels.Volume.Rotary;
using Prover.Shared;
using Prover.Shared.Interfaces;
using ReactiveUI;

namespace Prover.Application.VerificationManager.Volume
{
    public class RotaryVolumeManager : AutomatedVolumeTestManagerBase
    {
        internal RotaryVolumeManager(ILogger<RotaryVolumeManager> logger,
            IDeviceSessionManager deviceManager,
            ITachometerService tachometerService,
            PulseOutputsListenerService pulseListenerService,
            IOutputChannel motorControl,
            RotaryVolumeViewModel rotaryVolume) : base(logger, deviceManager, tachometerService, pulseListenerService,
            motorControl, rotaryVolume)
        {
            TachometerService = tachometerService;

            RotaryVolume = rotaryVolume;
        }

        public RotaryVolumeViewModel RotaryVolume { get; }

        public int TargetUncorrectedPulses => RotaryVolume.DriveType.MaxUncorrectedPulses();

        public override async Task PublishCompleteInteraction() =>
            await DeviceInteractions.CompleteVolumeTest.Handle(this);

        public override async Task<CancellationToken> PublishStartInteraction() =>
            await DeviceInteractions.StartVolumeTest.Handle(this);

        //public override async Task RunFinishActions()
        //{
        //    Logger.LogDebug("Completing test...");

        //    PulseListenerService.Stop();
        //    UpdatePulseOutputTestCounts();

        //    await DeviceInteractions.CompleteVolumeTest.Handle(this);

        //    var endValues = await DeviceManager.GetItemValues();
        //    RotaryVolume.EndValues = DeviceManager.Device.DeviceType.GetGroupValues<VolumeItems>(endValues);
        //}

        //public override async Task RunStartActions()
        //{
        //    Logger.LogInformation(
        //        $"Starting {RotaryVolume.DriveType.InputType} test for {DeviceManager.Device.DeviceType}.");
        //    var startValues = await DeviceManager.GetItemValues();
        //    RotaryVolume.StartValues = DeviceManager.Device.DeviceType.GetGroupValues<VolumeItems>(startValues);

        //    var cancelToken = await DeviceInteractions.StartVolumeTest.Handle(this);
        //    cancelToken.Register(() =>
        //    {
        //        Logger.LogWarning("Cancelling test...");
        //        MotorControl.SignalStop();
        //        PulseListenerService.Stop();
        //    });

        //    await ExecuteTestAsync();
        //}

        protected override async Task ExecuteTestAsync()
        {
            Logger.LogDebug("Running automated volume test.");

            PulseListenerService.StartListening()
                .Where(p =>
                    p.Items.Units == PulseOutputUnitType.UncVol && p.PulseCount == TargetUncorrectedPulses)
                .Select(_ => Unit.Default)
                .InvokeCommand(InitiateTestCompletion)
                .DisposeWith(Cleanup);

            MotorControl.SignalStart();
        }
    }
}