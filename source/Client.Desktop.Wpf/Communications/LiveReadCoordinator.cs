using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Interactions;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Client.Desktop.Wpf.Communications
{
    public partial class LiveReadCoordinator
    {
        public static async Task<IObservable<ItemValue>> StartLiveReading(IDeviceSessionManager session,
            VerificationTestPointViewModel test, Action onSuccessfulCompletion = null, IScheduler scheduler = null)
        {
            var liveReader = new LiveReadCoordinator();

            if (test.Pressure != null)
            {
                var pressureItem = session.Device.DeviceType.GetLivePressureItem();
                liveReader.AddReadingStabilizer(pressureItem, test.Pressure.GetTotalGauge());
            }

            if (test.Temperature != null)
            {
                var tempItem = session.Device.DeviceType.GetLiveTemperatureItem();
                liveReader.AddReadingStabilizer(tempItem, test.Temperature.Gauge);
            }

            liveReader.RegisterCallback(onSuccessfulCompletion, scheduler);

            return await liveReader.Start(session);
        }
    }

    public partial class LiveReadCoordinator : ReactiveObject, ILiveReadHandler, IDisposable
    {
        private readonly Dictionary<Action, IScheduler> _callbacks = new Dictionary<Action, IScheduler>();
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();

        private readonly Dictionary<ItemMetadata, AveragedReadingStabilizer> _liveReadItems;

        private LiveReadCoordinator() => _liveReadItems = new Dictionary<ItemMetadata, AveragedReadingStabilizer>();

        public Dictionary<ItemMetadata, decimal> ItemTargets { get; } = new Dictionary<ItemMetadata, decimal>();

        public IObservable<ItemValue> LiveReadUpdates { get; set; }
        public DeviceType DeviceType { get; private set; }

        public void Dispose()
        {
            _cleanup?.Dispose();
        }

        public async Task<IObservable<ItemValue>> Start(IDeviceSessionManager deviceSession)
        {
            await deviceSession.Connect();
            DeviceType = deviceSession.Device.DeviceType;
            var cancellationToken =
                await DeviceInteractions.LiveReading.Handle(this);

            var live = CreateLiveItemReadObservable(deviceSession, cancellationToken).Publish();

            live.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(
                    value => { },
                    async () => //OnComplete
                    {
                        Dispose();

                        await Task.Delay(500);
                        await deviceSession.Disconnect();

                        if (!cancellationToken.IsCancellationRequested) InvokeCallbacks();
                    })
                .DisposeWith(_cleanup);

            LiveReadUpdates = live.AsObservable();

            live.Connect()
                .DisposeWith(_cleanup);

            return LiveReadUpdates;
        }

        private void AddReadingStabilizer(ItemMetadata item, decimal targetValue)
        {
            _liveReadItems.Add(item, new AveragedReadingStabilizer(targetValue));
            ItemTargets.Add(item, targetValue);
        }

        private IObservable<ItemValue> CreateLiveItemReadObservable(IDeviceSessionManager deviceSession,
            CancellationToken cancellationToken)
        {
            return Observable.Create<ItemValue>(obs =>
            {
                var liveRead = TaskPoolScheduler.Default.SchedulePeriodic(TimeSpan.FromMilliseconds(250), () =>
                {
                    _liveReadItems.Keys.ForEach(async i =>
                    {
                        var value = await deviceSession.LiveReadItemValue(i);

                        _liveReadItems[value.Metadata].Add(value.DecimalValue() ?? 0m);

                        obs.OnNext(value);
                    });

                    if (_liveReadItems.All(i => i.Value.CheckIfStable()) || cancellationToken.IsCancellationRequested)
                        obs.OnCompleted();
                });

                return liveRead;
            });
        }

        private void InvokeCallbacks()
        {
            _callbacks.ForEach(kv => kv.Value?.Schedule(kv.Key));
        }

        private void RegisterCallback(Action callbackAction, IScheduler scheduler = null)
        {
            _callbacks.Add(callbackAction, scheduler ?? CurrentThreadScheduler.Instance);
        }

        private async Task StopLiveReading()
        {
            Dispose();
            await Task.Delay(500);
        }
    }
}