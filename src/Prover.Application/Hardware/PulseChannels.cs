using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Prover.Application.ViewModels;
using Prover.Shared;
using Prover.Shared.Interfaces;

namespace Prover.Application.Hardware
{
    public class SimulatorPulseChannelFactory : IInputChannelFactory, IOutputChannelFactory
    {
        public IInputChannel CreateInputChannel(PulseOutputChannel pulseChannel) => throw new NotImplementedException();

        public IOutputChannel CreateOutputChannel(OutputChannelType channelType) => throw new NotImplementedException();
    }

    public class SimulatedOutputChannel : IOutputChannel
    {
        private static ILogger _logger = ProverLogging.CreateLogger("SimulatedOutputChannel");
        public static Dictionary<OutputChannelType, SimulatedOutputChannel> OutputSimulators =
            new Dictionary<OutputChannelType, SimulatedOutputChannel>
            {
                {
                    OutputChannelType.Motor, new SimulatedOutputChannel(OutputChannelType.Motor, SimulatedInputChannel.PulseInputSimulators.Values)
                },
                {
                    OutputChannelType.Tachometer, new SimulatedOutputChannel(OutputChannelType.Tachometer)
                }
            };

        private readonly OutputChannelType _channel;
        private readonly IEnumerable<SimulatedInputChannel> _inputChannels ;

        public SimulatedOutputChannel(OutputChannelType channel, IEnumerable<SimulatedInputChannel> inputChannels= null)
        {
            _channel = channel;
            _inputChannels = inputChannels ?? new List<SimulatedInputChannel>();
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
        }
    }

    public class SimulatedInputChannel : IInputChannel, IDisposable
    {
        private static ILogger _logger = ProverLogging.CreateLogger("SimulatedOutputChannel");
        public PulseOutputChannel Channel { get; }

        public static Dictionary<PulseOutputChannel, SimulatedInputChannel> PulseInputSimulators =
            new Dictionary<PulseOutputChannel, SimulatedInputChannel>
            {
                {
                    PulseOutputChannel.Channel_A, new SimulatedInputChannel(PulseOutputChannel.Channel_A)
                },
                {
                    PulseOutputChannel.Channel_B, new SimulatedInputChannel(PulseOutputChannel.Channel_B)
                }
            };

        private readonly IScheduler _scheduler;
        private CompositeDisposable _cleanup;

        private int _currentValue = 255;
        private IObservable<int> _simulator;

        public SimulatedInputChannel(PulseOutputChannel channel, IScheduler scheduler = null)
        {
            Channel = channel;
            _scheduler = scheduler ?? TaskPoolScheduler.Default;
        }

        public int OffValue => 255;

        public double PulseIntervalSeconds => 2;

        public void Dispose()
        {
            _cleanup?.Dispose();
        }

        public IObservable<int> GetRandomSimulator(int? pulseIntervalMs = null)
        {
            var ticks = TimeSpan.FromMilliseconds(62.5).Ticks;

            return Observable.Create<int>(obs =>
            {
                _currentValue = 255;
                var random = new Random();
                var onValue = 0;

                var randomInterval = Observable.Generate(
                        onValue, x => true,
                        x => OffValue - _currentValue,
                        x => pulseIntervalMs ?? random.Next(500, 3000),
                        x => TimeSpan.FromMilliseconds(x), _scheduler)
                    .LogDebug($"Generated Pulse", _logger, true)
                    .Select(_ => onValue)
                    .Publish();

                var onOffSwitch = randomInterval
                    .Where(x => x == onValue)
                    .Timestamp()
                    .Delay(TimeSpan.FromTicks(ticks))
                    .Select(_ => OffValue);

                var cleanup = randomInterval.Merge(onOffSwitch).DelaySubscription(TimeSpan.FromSeconds(1))
                    .Do(x => _currentValue = x)
                    .Subscribe(obs.OnNext);

                return new CompositeDisposable(cleanup, randomInterval.Connect());
            });
        }

        public int GetValue() => _currentValue;

        public void Start()
        {
            _simulator = GetRandomSimulator();
            
            _cleanup = new CompositeDisposable(_simulator.Subscribe());
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