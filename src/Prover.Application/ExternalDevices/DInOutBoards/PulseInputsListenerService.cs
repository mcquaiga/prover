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
using Prover.Shared;
using ReactiveUI;

namespace Prover.Application.ExternalDevices.DInOutBoards
{
    public interface ISchedulerProvider
    {
        IScheduler CurrentThread { get; }
        IScheduler Dispatcher { get; }
        IScheduler Immediate { get; }
        IScheduler NewThread { get; }

        IScheduler ThreadPool { get; }
//IScheduler TaskPool { get; } 
    }

    public sealed class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler CurrentThread => Scheduler.CurrentThread;

        public IScheduler Dispatcher => RxApp.MainThreadScheduler;

        public IScheduler Immediate => Scheduler.Immediate;

        public IScheduler NewThread => NewThreadScheduler.Default;

        public IScheduler ThreadPool => ThreadPoolScheduler.Instance;

        //public IScheduler TaskPool { get { return Scheduler.TaskPool; } } 
    }

    public class PulseInputsListenerService
    {
        private readonly IInputChannelFactory _channelFactory;
        private CancellationTokenSource _cancellation;
        private CompositeDisposable _cleanup = new CompositeDisposable();
        private ICollection<PulseChannelListener> _inputChannels = new List<PulseChannelListener>();
        private ILogger _logger;
        private IScheduler _scheduler;

        public PulseInputsListenerService(ILoggerFactory loggerFactory, IInputChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
            _logger = loggerFactory.CreateLogger(typeof(PulseInputsListenerService));
        }

        public void Initialize(PulseOutputItems pulseOutputItems)
        {
            _inputChannels =
                pulseOutputItems.Channels
                    .Where(p => p.Units == PulseOutputUnitType.CorVol || p.Units == PulseOutputUnitType.UncVol)
                    .Select(pout => new PulseChannelListener(pout, _channelFactory.CreateInputChannel(pout.Name)))
                    .ToList();
        }

        public void StartListening()
        {
            if (_inputChannels.Count == 0)
                throw new NullReferenceException(
                    "Initialize must be called before StartListening on PulseInputsListenerService.");

            var pulses = _inputChannels.Select(c => c.PulseListener.Connect());

            _cleanup = new CompositeDisposable(
                pulses.Select(p =>
                    p.Subscribe()
                )
            );
        }

        private IObservable<IChangeSet<PulseChannelListener, PulseOutputChannel>> PulseCheckerObservable()
        {
            _cancellation = new CancellationTokenSource();

            return ObservableChangeSet.Create<PulseChannelListener, PulseOutputChannel>(cache =>
            {
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

                var recurring = 
                    _inputChannels.ForEach(i =>
                        _scheduler.ScheduleRecurringAction(TimeSpan.FromMilliseconds(15.625), () =>
                        {
                            var newPulse = i.CheckForPulse();
                            if (newPulse)
                                cache.AddOrUpdate(i);
                        }));

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

        public PulseChannelListener(PulseOutputItems.ChannelItems items, IInputChannel channel,
            ISchedulerProvider scheduler = null)
        {
            Items = items;
            Channel = channel;

            scheduler = scheduler ?? new SchedulerProvider();

            _scheduler = scheduler;

            OffValue = Channel.GetValue();

            //_listener = ChannelListenerObservable().Publish();
            ////var listener = ChannelListenerObservable()
            ////    //.ObserveOn(scheduler.Dispatcher)
            ////    .Publish();

            //PulseListener = _listener.AsObservableList();

            //listener.Count()
            //    .Subscribe(x => TotalPulses = x);
            //_listener = listener;
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
            Debug.WriteLine($"Pulse Channel {Name} - Value = {pulseOn}");

            if (pulseOn && !_previousPulseOn)
            {
                TotalPulses++;
                Debug.WriteLine($"Pulse Channel {Name} = {TotalPulses} pulses");

                _previousPulseOn = true;
                return true;
            }

            _previousPulseOn = false;
            return false;
        }

        public async Task<bool> CheckForPulseAsync(CancellationToken token)
        {
            return await Task.Run(CheckForPulse, token);
        }

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
                    //var listen = poolScheduler.ScheduleLongRunning(cancel =>
                    //{
                    //    do
                    //    {
                    //        ListenForPulses();
                    //    } while (true);

                    //});
                    //cleanup = new CompositeDisposable(cleanup, listen);
                    //TaskPoolScheduler.Default
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
                        Debug.WriteLine($"Pulse Channel {Name} - Value = {value}");
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