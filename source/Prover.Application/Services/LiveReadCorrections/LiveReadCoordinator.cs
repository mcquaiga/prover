using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Services.LiveReadCorrections
{
    public partial class LiveReadCoordinator
    {
        public static async Task<IObservable<ItemLiveReadStatus>> StartLiveReading(IDeviceSessionManager session,
            VerificationTestPointViewModel test, Action onSuccessfulCompletion = null, IScheduler scheduler = null)
        {
            var liveReader = new LiveReadCoordinator(session);

            if (test.Pressure != null)
            {
                var pressureItem = session.Device.GetLivePressureItem();
                liveReader.AddReadingStabilizer(pressureItem, test.Pressure.GetTotalGauge());
            }

            if (test.Temperature != null)
            {
                var tempItem = session.Device.GetLiveTemperatureItem();
                liveReader.AddReadingStabilizer(tempItem, test.Temperature.Gauge);
            }

            liveReader.RegisterCallback(onSuccessfulCompletion, scheduler);

            return await liveReader.Start();
        }
    }

    public class ItemLiveReadStatus
    {
        private readonly Subject<ItemLiveReadStatus> _statusSubject = new Subject<ItemLiveReadStatus>();

        public ItemLiveReadStatus(ItemMetadata item, AveragedReadingStabilizer stabilizer)
        {
            Item = item;
            Stabilizer = stabilizer;
            Value = ItemValue.Create(Item, 0m);
        }

        public IObservable<ItemLiveReadStatus> StatusUpdates => _statusSubject;
        public ItemMetadata Item { get; }

        public AveragedReadingStabilizer Stabilizer { get; }

        public decimal TargetValue => Stabilizer.TargetValue;

        public ItemValue Value { get; private set; }

        public void AddUpdate(ItemValue value)
        {
            Value = value;
            if (value.DecimalValue().HasValue)
            {
                Stabilizer.Add(value.DecimalValue() ?? 0m);
                _statusSubject.OnNext(this);
            }
        }
    }

    public partial class LiveReadCoordinator : ILiveReadHandler, IDisposable
    {
        private readonly Dictionary<Action, IScheduler> _callbacks = new Dictionary<Action, IScheduler>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly IDeviceSessionManager _deviceSession;
        private readonly IConnectableObservable<ItemLiveReadStatus> _liveStatuses;

        public LiveReadCoordinator(IDeviceSessionManager deviceSession)
        {
            _deviceSession = deviceSession;
            _liveStatuses = CreateLiveItemReadObservable(deviceSession, _cancellationTokenSource.Token).Publish();
        }


        public ICollection<ItemLiveReadStatus> LiveReadItems { get; set; } = new List<ItemLiveReadStatus>();

        public IObservable<ItemLiveReadStatus> LiveReadUpdates { get; private set; }

        public DeviceType DeviceType => _deviceSession.Device?.DeviceType;

        public void Dispose()
        {
            _cleanup?.Dispose();
        }

        public async Task<IObservable<ItemLiveReadStatus>> Start()
        {
            await _deviceSession.Connect();

            var cancellationToken =
                await DeviceInteractions.LiveReading.Handle(this);

            cancellationToken.Register(_cancellationTokenSource.Cancel);

            //var live = CreateLiveItemReadObservable(_deviceSession, cancellationToken).Publish();

            _liveStatuses.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(
                    value => { },
                    async () => //OnComplete
                    {
                        Dispose();

                        await Task.Delay(500);
                        await _deviceSession.Disconnect();

                        if (!cancellationToken.IsCancellationRequested) InvokeCallbacks();
                    })
                .DisposeWith(_cleanup);

            LiveReadUpdates = _liveStatuses.AsObservable();

            _liveStatuses
                .Connect()
                .DisposeWith(_cleanup);

            return LiveReadUpdates;
        }

        public async Task Stop()
        {
            Dispose();
            await Task.Delay(500);
        }

        private void AddReadingStabilizer(ItemMetadata item, decimal targetValue)
        {
            LiveReadItems.Add(new ItemLiveReadStatus(item, new AveragedReadingStabilizer(targetValue)));
        }

        private IObservable<ItemLiveReadStatus> CreateLiveItemReadObservable(IDeviceSessionManager deviceSession,
            CancellationToken cancellationToken)
        {
            return Observable.Create<ItemLiveReadStatus>(obs =>
            {
                var liveRead = TaskPoolScheduler.Default.SchedulePeriodic(TimeSpan.FromMilliseconds(250), () =>
                {
                    LiveReadItems.ForEach(async i =>
                    {
                        var value = await deviceSession.LiveReadItemValue(i.Item);
                        i.AddUpdate(value);
                        obs.OnNext(i);
                    });

                    if (LiveReadItems.All(i => i.Stabilizer.IsStable) || cancellationToken.IsCancellationRequested)
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
    }
}