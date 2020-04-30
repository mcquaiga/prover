using Microsoft.Extensions.Logging;
using Prover.Application.Hardware;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels.Volume.Rotary;
using Prover.Shared;
using Prover.Shared.Interfaces;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Application.Verifications.Volume
{
    public class RotaryVolumeTestRunner : AutomatedVolumeTestRunnerBase
    {
        internal RotaryVolumeTestRunner(ILogger<RotaryVolumeTestRunner> logger,
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

        public override int TargetUncorrectedPulses
        {
            get
            {
                switch (DeviceManager.Device.Items.Volume.UncorrectedMultiplier)
                {
                    case 10:
                        return RotaryVolume.RotaryMeterTest.Items.MeterType.UnCorPulsesX10;
                    case 100:
                        return RotaryVolume.RotaryMeterTest.Items.MeterType.UnCorPulsesX100;
                    default:
                        return 10;
                }
            }
        }

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

            await Task.CompletedTask;
        }
    }
}