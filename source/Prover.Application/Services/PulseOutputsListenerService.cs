using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Shared;
using Prover.Shared.Interfaces;
using ReactiveUI;
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

namespace Prover.Application.Services {
	public class PulseOutputsListenerService : IDisposable {
		private const double CheckIntervalTime = 20;
		private readonly IScheduler _background;
		private readonly Func<PulseOutputChannel, IInputChannel> _channelFactoryFunc;
		private readonly CompositeDisposable _fullCleanup = new CompositeDisposable();
		private readonly CompositeDisposable _stopCleanup = new CompositeDisposable();
		private CancellationTokenSource _cancellation;
		private ICollection<PulseChannelListener> _inputChannels = new List<PulseChannelListener>();
		private IConnectableObservable<PulseChannel> _listeners;

		private ILogger _logger;
		//private ICompleteVolumeTest _testTerminator;

		private PulseOutputsListenerService(ILoggerFactory loggerFactory, IScheduler backgroundThread = null) {
			_logger = loggerFactory?.CreateLogger(typeof(PulseOutputsListenerService)) ?? NullLogger.Instance;
			_background = backgroundThread ?? RxApp.TaskpoolScheduler;

			SetupListeners();

			_stopCleanup.DisposeWith(_fullCleanup);
		}

		public PulseOutputsListenerService(ILoggerFactory loggerFactory, IInputChannelFactory channelFactory,
			IScheduler backgroundThread = null) : this(loggerFactory, backgroundThread)
			=> _channelFactoryFunc = channelFactory.CreateInputChannel;

		public PulseOutputsListenerService(ILoggerFactory loggerFactory,
			Func<PulseOutputChannel, IInputChannel> channelFactoryFunc, IScheduler backgroundThread = null)
			: this(loggerFactory, backgroundThread)
			=> _channelFactoryFunc = channelFactoryFunc;

		public IObservable<PulseChannel> PulseCountUpdates { get; set; }

		public ICollection<PulseChannel> PulseChannels { get; private set; } = new List<PulseChannel>();

		public void Dispose() {
			_fullCleanup?.Dispose();
		}

		public void Initialize(PulseOutputItems pulseOutputItems) {
			if (pulseOutputItems?.Channels == null || pulseOutputItems.Channels.Count == 0)
				throw new NullReferenceException(nameof(pulseOutputItems));

			if (_inputChannels.Any())
				return;

			_inputChannels = pulseOutputItems.Channels
				.Where(p => p.ChannelType == PulseOutputType.CorVol || p.ChannelType == PulseOutputType.UncVol)
				.Select(pout => new PulseChannelListener(pout, _channelFactoryFunc.Invoke(pout.Name)))
				.ToList();

			PulseChannels = _inputChannels?.Select(p => p.Pulser).ToList();
		}

		public IObservable<PulseChannel> StartListening() {
			if (!_inputChannels.Any())
				throw new Exception("Pulse input channels haven't been configured. Call Initialize first.");

			_listeners.Connect()
				.DisposeWith(_stopCleanup);

			return PulseCountUpdates;
		}

		public void Stop() {
			_cancellation.Cancel();
			_stopCleanup?.Dispose();
			SetupListeners();
		}

		public void ResetCounts() => _inputChannels.ForEach(i => i.Pulser.PulseCount = 0);

		private IObservable<PulseChannel> PulseCheckerObservable() {
			return Observable.Create<PulseChannel>(observer => {
				var cleanup = new CompositeDisposable();

				_inputChannels.ForEach(channel => {
					channel.ListenForPulses(observer, _cancellation);
				});
				//_background.SchedulePeriodic(TimeSpan.FromMilliseconds(CheckIntervalTime), () => {
				//	_inputChannels.ForEach(async channel => {
				//		await Task.Run(() => {
				//			   	if (channel.CheckForPulse())
				//			observer.OnNext(channel.Pulser);
				//		})	;

				//	});
				//}).DisposeWith(cleanup);
				//_background.Schedule(() => {
				//	while (!_cancellation.IsCancellationRequested) {
				//		foreach (var input in _inputChannels) {
				//			if (input.CheckForPulse())
				//				observer.OnNext(input.Pulser);
				//		}
				//	}
				//}).DisposeWith(cleanup);
				//_background.AsLongRunning().ScheduleLongRunning(() => {
				//	while (true) {
				//		foreach (var input in _inputChannels) {
				//			if (input.CheckForPulse())
				//				observer.OnNext(input.Pulser);
				//		}
				//	}
				//	return _cancellation;
				//}).DisposeWith(cleanup);


				//return cleanup;

				// ** Working **
				//_inputChannels.ForEach(channel => {
				//	Task.Run(() => {
				//		while (!_cancellation.IsCancellationRequested) {
				//			if (channel.CheckForPulse())
				//				observer.OnNext(channel.Pulser);
				//		}
				//	});
				//});

				return cleanup;
			});
		}

		private void SetupListeners() {
			_cancellation = new CancellationTokenSource();
			//_cancellation.DisposeWith(_stopCleanup);

			_listeners = PulseCheckerObservable().Publish();
			PulseCountUpdates = _listeners.AsObservable();

			PulseCountUpdates
				.Subscribe()
				.DisposeWith(_stopCleanup);
		}

		#region Nested type: PulseChannelListener

		private class PulseChannelListener : IDisposable {
			private readonly IInputChannel _inputChannel;
			private readonly int _offValue;
			private bool _previousPulseOn = false;

			public PulseChannelListener(PulseOutputItems.ChannelItems items, IInputChannel channel) {
				Pulser = new PulseChannel {
					Items = items,
					Channel = items.Name,
					PulseCount = 0
				};

				_inputChannel = channel;

				_offValue = _inputChannel.GetValue();
			}

			public PulseChannel Pulser { get; }

			public bool CheckForPulse() {
				var pulseOn = _inputChannel.GetValue() != _offValue;

				if (pulseOn && !_previousPulseOn) {
					_previousPulseOn = true;

					Pulser.PulseCount++;
					Debug.WriteLine($"[{Pulser.Channel}] = {Pulser.PulseCount}");

					return true;
				}

				_previousPulseOn = pulseOn;
				return false;
			}

			public Task ListenForPulses(IObserver<PulseChannel> observer, CancellationTokenSource _cancellation) {
				return Task.Run(async () => {
					while (!_cancellation.IsCancellationRequested) {
						if (CheckForPulse()) {
							observer.OnNext(Pulser);
						}
						await Task.Delay(32);
					}
				}, _cancellation.Token);
			}

			public async Task<bool> CheckForPulseAsync(CancellationToken token) => await Task.Run(CheckForPulse, token);

			public void Dispose() {
			}
		}

		#endregion
	}

	public class PulseChannel {
		public PulseOutputChannel Channel { get; set; }

		public int PulseCount { get; set; }

		public PulseOutputItems.ChannelItems Items { get; set; }
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