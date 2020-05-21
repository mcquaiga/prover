using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application;
using Prover.Shared;
using Prover.Shared.Interfaces;

namespace Prover.Modules.DevTools.Hardware
{
    public class SimulatorPulseChannelFactory : IInputChannelFactory, IOutputChannelFactory
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;

        public SimulatorPulseChannelFactory(ILogger logger = null, IScheduler scheduler = null)
        {
            _logger = logger;
            _scheduler = scheduler;
        }

        public IInputChannel CreateInputChannel(PulseOutputChannel pulseChannel) =>
            SimulatedInputChannel.PulseInputSimulators[pulseChannel];

        public IOutputChannel CreateOutputChannel(OutputChannelType channelType) =>
            SimulatedOutputChannel.OutputSimulators[channelType];
    }

    public class SimulatedOutputChannel : IOutputChannel
    {
        private static Dictionary<OutputChannelType, SimulatedOutputChannel> _outputSimulators;

        private readonly OutputChannelType _channel;
        private readonly ICollection<SimulatedInputChannel> _inputChannels = new List<SimulatedInputChannel>();
        private readonly ILogger _logger;

        public SimulatedOutputChannel(OutputChannelType channel, ICollection<SimulatedInputChannel> inputChannels,
            ILogger logger = null) : this(channel, logger)
            => _inputChannels = inputChannels ?? new List<SimulatedInputChannel>();

        public SimulatedOutputChannel(OutputChannelType channel, ILogger logger = null)
        {
            _channel = channel;
            _logger = logger ?? NullLogger.Instance;
        }

        public static Dictionary<OutputChannelType, SimulatedOutputChannel> OutputSimulators
        {
            get
            {
                if (_outputSimulators == null)
                    _outputSimulators = new Dictionary<OutputChannelType, SimulatedOutputChannel>
                    {
                        {
                            OutputChannelType.Motor,
                            new SimulatedOutputChannel(OutputChannelType.Motor,
                                SimulatedInputChannel.PulseInputSimulators.Values)
                        },
                        {
                            OutputChannelType.Tachometer, new SimulatedOutputChannel(OutputChannelType.Tachometer)
                        }
                    };

                return _outputSimulators;
            }
        }

        public void OutputValue(short value)
        {
            _logger.LogDebug($"Output channel -> value {value} sent.");
        }

        public void SignalStart()
        {
            _logger.LogDebug("START Signal called on output channel.");
            _inputChannels?.ForEach(i => i.Start());
        }

        public void SignalStop()
        {
            _logger.LogDebug("Signal Stop called on output channel.");
            _inputChannels.First(p => p.Channel == PulseOutputChannel.Channel_A).Stop();

            Scheduler.Schedule((IScheduler)TaskPoolScheduler.Default, (TimeSpan)TimeSpan.FromSeconds(15),
                () => _inputChannels.First(p => p.Channel == PulseOutputChannel.Channel_B).Stop());
        }
    }

    public class SimulatedInputChannel : IInputChannel, IDisposable
    {
        public static readonly Dictionary<PulseOutputChannel, SimulatedInputChannel> PulseInputSimulators =
            new Dictionary<PulseOutputChannel, SimulatedInputChannel>
            {
                {PulseOutputChannel.Channel_A, new SimulatedInputChannel(PulseOutputChannel.Channel_A, 1000)},
                {PulseOutputChannel.Channel_B, new SimulatedInputChannel(PulseOutputChannel.Channel_B)}
            };

        private readonly int? _intervalMs;

        private readonly ILogger _logger;

        private readonly IScheduler _scheduler;
        private CompositeDisposable _cleanup;

        private int _currentValue;

        public SimulatedInputChannel(PulseOutputChannel channel, int? intervalMs = null, ILogger logger = null,
            IScheduler scheduler = null)
        {
            _intervalMs = intervalMs;
            _logger = logger ?? ProverLogging.CreateLogger("SimulatedInputChannel");
            Channel = channel;
            _scheduler = scheduler ?? TaskPoolScheduler.Default;

            _currentValue = OffValue;
        }

        public PulseOutputChannel Channel { get; }

        public int OffValue => 255;

        public double PulseIntervalSeconds => 2;

        public void Dispose()
        {
            _cleanup?.Dispose();
        }

        public IObservable<int> GetRandomSimulator(int? pulseIntervalMs = null)
        {
            pulseIntervalMs = pulseIntervalMs ?? _intervalMs;
            var pulseSpan = TimeSpan.FromMilliseconds(62.5);

            var cleanup = new CompositeDisposable();

            const int onValue = 0;

            return Observable.Create<int>(obs =>
            {
                var random = new Random();

                return Observable.Interval(TimeSpan.FromMilliseconds(pulseIntervalMs ?? random.Next(300, 3000)), _scheduler)
                    .Do(_ => obs.OnNext(onValue))
                    .Select(_ => Observable.Timer(pulseSpan, _scheduler))
                    .Do(span => span.Subscribe(_ => obs.OnNext(OffValue)))
                    .Publish()
                    .Connect();
            });
        }

        public int GetValue() => _currentValue;

        public void Start()
        {
            _cleanup = new CompositeDisposable();

            GetRandomSimulator(_intervalMs)
                .Subscribe(x => _currentValue = x)
                .DisposeWith(_cleanup);
        }

        public void Stop()
        {
            _cleanup.Dispose();
        }
    }
}

//public IObservable<int> GetSimulator()
//{
//    _currentValue = 255;
//    return Observable.Create<int>(obs =>
//    {
//        var interval = Observable.Interval(TimeSpan.FromSeconds(PulseIntervalSeconds))
//            .Do(t => Debug.WriteLine($"Interval - {t} ms"))
//            .Select(_ => OffValue - _currentValue)
//            .Publish();

//        var onOff = interval
//            .Where(x => x != OffValue)
//            .Delay(TimeSpan.FromMilliseconds(62.5))
//            .Select(x => OffValue);

//        var cleanup2 = interval.Merge(onOff)
//            .Do(x => Debug.WriteLine($"Value = {x}"))
//            .Do(newValue => _currentValue = newValue)
//            .Subscribe(obs.OnNext);

//        return new CompositeDisposable(cleanup2, interval.Connect());
//    });
//}