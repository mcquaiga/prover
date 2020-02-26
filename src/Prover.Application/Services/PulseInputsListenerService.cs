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
        private readonly Func<PulseOutputChannel, IInputChannel> _channelFactoryFunc;
        private readonly IConnectableObservable<IChangeSet<PulseChannel, PulseOutputChannel>> _listeners;
        private CancellationTokenSource _cancellation;
        private CompositeDisposable _cleanup;
        private ICollection<PulseChannelListener> _inputChannels = new List<PulseChannelListener>();
        private ILogger _logger;

        private PulseInputsListenerService(ILoggerFactory loggerFactory, IScheduler backgroundThread = null)
        {
            _logger = loggerFactory?.CreateLogger(typeof(PulseInputsListenerService)) ?? NullLogger.Instance;
            _background = backgroundThread ?? TaskPoolScheduler.Default;


            _listeners = PulseCheckerObservable().Publish();
            PulseListener = _listeners.AsObservableCache();

            _cleanup = new CompositeDisposable(PulseListener);
        }

        public PulseInputsListenerService(
            ILoggerFactory loggerFactory, 
            IInputChannelFactory channelFactory,
            IScheduler backgroundThread = null

        ) : this(loggerFactory, backgroundThread) 
            => _channelFactoryFunc = channelFactory.CreateInputChannel;

        public PulseInputsListenerService(
            ILoggerFactory loggerFactory,
            Func<PulseOutputChannel, IInputChannel> channelFactoryFunc, 
            IScheduler backgroundThread = null

        ) : this(loggerFactory, backgroundThread) 
            => _channelFactoryFunc = channelFactoryFunc;

        public IObservableCache<PulseChannel, PulseOutputChannel> PulseListener { get; }

        public void Dispose()
        {
            _cancellation?.Dispose();
            _cleanup?.Dispose();
        }

        public void Initialize(PulseOutputItems pulseOutputItems)
        {
            if (pulseOutputItems?.Channels == null || pulseOutputItems.Channels.Count == 0)
                throw new NullReferenceException(nameof(pulseOutputItems));

            _inputChannels = pulseOutputItems.Channels
                .Where(p => p.Units == PulseOutputUnitType.CorVol || p.Units == PulseOutputUnitType.UncVol)
                .Select(pout => new PulseChannelListener(pout, _channelFactoryFunc.Invoke(pout.Name)))
                .ToList();
        }

        public IObservableCache<PulseChannel, PulseOutputChannel> StartListening()
        {
            if (!_inputChannels.Any())
                throw new Exception("Pulse input channels haven't been configured. Call Initialize first.");

            _cleanup = new CompositeDisposable(_cleanup, _listeners.Connect());

            return PulseListener;
        }

        private IObservable<IChangeSet<PulseChannel, PulseOutputChannel>> PulseCheckerObservable()
        {
            _cancellation = new CancellationTokenSource();

            return ObservableChangeSet.Create<PulseChannel, PulseOutputChannel>(cache =>
            {
                var recurring = _inputChannels.ForEach(i =>
                {
                    cache.AddOrUpdate(i.Pulser);
                    _background.ScheduleRecurringAction(TimeSpan.FromMilliseconds(CheckIntervalTime), () =>
                    {
                        var newPulse = i.CheckForPulse();
                        if (newPulse)
                            cache.AddOrUpdate(i.Pulser);
                    });
                });


                return new CompositeDisposable(recurring);
            }, pulser => pulser.Channel);
        }
    }

    public class PulseChannel
    {
        public PulseOutputChannel Channel { get; set; }

        public int PulseCount { get; set; }

        public PulseOutputItems.ChannelItems Items { get; set; }
    }

    public class PulseChannelListener : IDisposable
    {
        private CancellationTokenSource _cancellation;
        private readonly CompositeDisposable _cleanup;

        private bool _previousPulseOn;

        protected IInputChannel Channel;
        //private readonly IObservableList<int> _pulses;

        //public IObservableList<int> Pulses => _pulses;
        // We expose the Connect() since we are interested in a stream of changes.
        // If we have more than one subscriber, and the subscribers are known, 
        // it is recommended you look into the Reactive Extension method Publish().

        public PulseChannelListener(PulseOutputItems.ChannelItems items, IInputChannel channel)
        {
            Pulser = new PulseChannel
            {
                Items = items, 
                Channel = items.Name, 
                PulseCount = 0
            };

            Channel = channel;

            OffValue = Channel.GetValue();

            _cleanup = new CompositeDisposable();
        }

        public PulseChannel Pulser { get; set; }

        public int OffValue { get; }

        //public IObservableList<int> PulseListener { get; set; }

        public bool CheckForPulse()
        {
            var pulseOn = Channel.GetValue() != OffValue;
            Debug.WriteLine($"[{Pulser.Channel}] = {pulseOn}");

            if (pulseOn && !_previousPulseOn)
            {
                _previousPulseOn = true;

                Pulser.PulseCount++;

                Debug.WriteLine($"[{Pulser.Channel}] = {Pulser.PulseCount}");

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