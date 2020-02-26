using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;
using DynamicData;
using DynamicData.Kernel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Shared;
using Prover.Shared.Interfaces;

namespace Prover.Application.Services
{
    public class PulseInputsListenerService : IDisposable
    {
        private const double CheckIntervalTime = 31.25;
        private readonly IScheduler _background;
        private readonly IInputChannelFactory _channelFactory;
        private CancellationTokenSource _cancellation;
        private CompositeDisposable _cleanup = new CompositeDisposable();
        private ICollection<PulseChannelListener> _inputChannels = new List<PulseChannelListener>();
        private ILogger _logger;
        private IConnectableObservable<IChangeSet<PulseChannelListener, PulseOutputChannel>> _listeners;

        public PulseInputsListenerService(ILoggerFactory loggerFactory, IInputChannelFactory channelFactory,
            IScheduler backgroundThread)
        {
            _channelFactory = channelFactory;
            _logger = loggerFactory?.CreateLogger(typeof(PulseInputsListenerService)) ?? NullLogger.Instance;

            _background = backgroundThread;

            _listeners = PulseCheckerObservable().Publish();

            PulseListener = _listeners.AsObservableCache();
        }

        public void Dispose()
        {
            _cancellation?.Dispose();
            _cleanup?.Dispose();
        }

        public IObservableCache<PulseChannelListener, PulseOutputChannel> StartListening(PulseOutputItems pulseOutputItems)
        {
            if (pulseOutputItems?.Channels == null || pulseOutputItems.Channels.Count == 0)
                throw new NullReferenceException(nameof(pulseOutputItems));

            _inputChannels = pulseOutputItems.Channels
                .Where(p => p.Units == PulseOutputUnitType.CorVol || p.Units == PulseOutputUnitType.UncVol)
                .Select(pout => new PulseChannelListener(pout, _channelFactory.CreateInputChannel(pout.Name)))
                .ToList();

            _cleanup = new CompositeDisposable(_listeners.Connect());

            return PulseListener;
        }

        public IObservableCache<PulseChannelListener, PulseOutputChannel> PulseListener { get; set; }

        private IObservable<IChangeSet<PulseChannelListener, PulseOutputChannel>> PulseCheckerObservable()
        {
            _cancellation = new CancellationTokenSource();

            return ObservableChangeSet.Create<PulseChannelListener, PulseOutputChannel>(cache =>
            {
                var recurring = _inputChannels.ForEach(i =>
                    {
                        cache.AddOrUpdate(i);
                        _background.ScheduleRecurringAction(TimeSpan.FromMilliseconds(CheckIntervalTime), () =>
                        {
                            var newPulse = i.CheckForPulse();
                            if (newPulse)
                                cache.AddOrUpdate(i);
                        });
                    });
                       
                        

                return new CompositeDisposable(recurring);
            }, listener => listener.Name);
        }
    }

    public class PulseChannelListener : IDisposable
    {
        private readonly IConnectableObservable<IChangeSet<int>> _listener;
        private readonly ISchedulerProvider _scheduler;

        private CancellationTokenSource _cancellation;
        private CompositeDisposable _cleanup;

        private bool _previousPulseOn;
        //private readonly IObservableList<int> _pulses;

        //public IObservableList<int> Pulses => _pulses;
        // We expose the Connect() since we are interested in a stream of changes.
        // If we have more than one subscriber, and the subscribers are known, 
        // it is recommended you look into the Reactive Extension method Publish().

        public PulseChannelListener(PulseOutputItems.ChannelItems items, IInputChannel channel
        )
        {
            Items = items;
            Channel = channel;

            OffValue = Channel.GetValue();
        }

        public int TotalPulses { get; private set; }

        public PulseOutputChannel Name => Items.Name;
        public PulseOutputItems.ChannelItems Items { get; }
        public IInputChannel Channel { get; }

        public int OffValue { get; }

        public IObservableList<int> PulseListener { get; set; }

        public bool CheckForPulse()
        {
            var pulseOn = Channel.GetValue() != OffValue;
            Debug.WriteLine($"[{Name}] = {pulseOn}");

            if (pulseOn && !_previousPulseOn)
            {
                TotalPulses++;
                Debug.WriteLine($"[{Name}] = {TotalPulses}");

                _previousPulseOn = true;
                return true;
            }

            _previousPulseOn = pulseOn;
            return false;
        }

        public async Task<bool> CheckForPulseAsync(CancellationToken token) => await Task.Run(CheckForPulse, token);

        public void Dispose()
        {
            _cleanup?.Dispose();
        }

        public IObservable<IChangeSet<int>> Start()
        {
            _cleanup = new CompositeDisposable(_listener.Connect());
            return _listener.AsObservable();
        }

        private IObservable<IChangeSet<int>> ChannelListenerObservable()
        {
            var offValue = Channel.GetValue();

            return ObservableChangeSet.Create<int>(obs =>
            {
                var isPulseCleared = true;
                var total = 0;
                var cleanup = new CompositeDisposable();

                _cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                if (_scheduler.ThreadPool is ThreadPoolScheduler poolScheduler)
                {
                 
                }
                else
                {
                    var listen2 = _scheduler.ThreadPool
                        .Schedule(() => ListenForPulses(_cancellation.Token));

                    cleanup = new CompositeDisposable(cleanup, listen2);
                }

                return cleanup;

                void ListenForPulses(CancellationToken token)
                {
                    do
                    {
                        var value = Channel.GetValue();
                        Debug.WriteLine($"Pulse Channel {Name} = {value}");
                        if (value != offValue)
                        {
                            if (isPulseCleared)
                            {
                                isPulseCleared = false;
                                total++;
                                obs.Add(total);
                                Debug.WriteLine($"Pulse Channel {Name} = {total} pulses");
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"Pulse Channel {Name} Pulse cleared = {isPulseCleared}");
                            isPulseCleared = true;
                        }
                    } while (!token.IsCancellationRequested);

                    Debug.WriteLine($"Cancellation requested {token.IsCancellationRequested}");
                }
            });
        }

        #region Nested type: PulseValue

        private enum PulseValue
        {
            On = 1,
            Off = 0
        }

        #endregion
    }
}

//var recurring = _scheduler.Schedule(() =>
//{
//    do
//    {
//        _inputChannels.ForEach(async i =>
//        {
//            var newPulse = await i.CheckForPulseAsync(_cancellation.Token);
//            if (newPulse)
//                cache.AddOrUpdate(i);
//        });
//    } while (!_cancellation.Token.IsCancellationRequested);
//});