using System;
using System.Reactive;
using System.Threading;
using Devices.Communications.Status;
using Prover.Application.Interfaces;
using Prover.Application.Verifications.Volume;
using Prover.Application.ViewModels.Volume.Rotary;
using ReactiveUI;

namespace Prover.Application.Interactions
{
    public static class DeviceInteractions
    {
        public static Interaction<IObservable<StatusMessage>, CancellationToken> Connecting { get; } =
            new Interaction<IObservable<StatusMessage>, CancellationToken>();

        public static Interaction<IDeviceSessionManager, CancellationToken> Disconnecting { get; } =
            new Interaction<IDeviceSessionManager, CancellationToken>();

        public static Interaction<IDeviceSessionManager, CancellationToken> DownloadingItems { get; } =
            new Interaction<IDeviceSessionManager, CancellationToken>();

        public static Interaction<ILiveReadHandler, CancellationToken> LiveReading { get; } =
            new Interaction<ILiveReadHandler, CancellationToken>();

        public static Interaction<IDeviceSessionManager, Unit> Unlinked { get; } =
            new Interaction<IDeviceSessionManager, Unit>();

        public static Interaction<IVolumeTestManager, CancellationToken> StartVolumeTest { get; } =
            new Interaction<IVolumeTestManager, CancellationToken>();

        public static Interaction<IVolumeTestManager, CancellationToken> CompleteVolumeTest { get; } =
            new Interaction<IVolumeTestManager, CancellationToken>();
    }
}