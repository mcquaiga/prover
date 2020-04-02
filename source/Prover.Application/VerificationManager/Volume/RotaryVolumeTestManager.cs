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
            IAppliedInputVolume tachometerService,
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
        
        protected override async Task ExecuteTestAsync()
        {
            Logger.LogDebug("Running automated volume test.");

            PulseListenerService.StartListening()
                .Where(p =>
                    p.Items.ChannelType == PulseOutputType.UncVol && p.PulseCount == TargetUncorrectedPulses)
                .Select(_ => Unit.Default)
                .InvokeCommand(InitiateTestCompletion)
                .DisposeWith(Cleanup);

            MotorControl.SignalStart();
        }
    }
}