using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;

namespace Prover.UI.Desktop.Common {
	public class CronSchedules {
		public static CronExpression Daily { get; } = CronExpression.Parse("0 0 * * *");
		public static CronExpression Hourly { get; } = CronExpression.Parse("0 * * * *");
		public static CronExpression Minute { get; } = CronExpression.Parse("* * * * *");
		//public static CronExpression Hours { get; } = CronExpression.Parse("0 */2 * * *");
	}

	public abstract class CronJobService : IHostedService, IDisposable {
		private System.Timers.Timer _timer;
		private readonly CronExpression _expression;
		private readonly TimeZoneInfo _timeZoneInfo;

		protected CronJobService(CronExpression cronExpression, TimeZoneInfo timeZoneInfo = null) {
			_expression = cronExpression;
			_timeZoneInfo = timeZoneInfo ?? TimeZoneInfo.Local;
		}

		protected CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo) {
			_expression = !string.IsNullOrEmpty(cronExpression) ? CronExpression.Parse(cronExpression) : null;
			_timeZoneInfo = timeZoneInfo;
		}

		public virtual async Task StartAsync(CancellationToken cancellationToken) {

			await ScheduleJob(cancellationToken);
			//return Task.CompletedTask;
		}

		protected virtual Task ScheduleJob(CancellationToken cancellationToken) {

			ObservableScheduler(cancellationToken);
			//ScheduleWithTimer(cancellationToken);

			return Task.CompletedTask;
			//await DoWork(cancellationToken);
			//await ScheduleJob(cancellationToken);


		}

		public abstract Task DoWork(CancellationToken cancellationToken);

		public virtual async Task StopAsync(CancellationToken cancellationToken) {
			_timer?.Stop();
			await Task.CompletedTask;
		}

		public virtual void Dispose() {
			_timer?.Dispose();
		}

		protected virtual IDisposable ObservableScheduler(CancellationToken token, IScheduler scheduler = null) {
			var disposer = new CompositeDisposable();
			scheduler = scheduler ?? TaskPoolScheduler.Default;

			DateTimeOffset? GetNext(DateTimeOffset? from = null) {

				return _expression.GetNextOccurrence(from ?? DateTimeOffset.Now, _timeZoneInfo);
			}

			IDisposable worker() => scheduler.Schedule(async () => await DoWork(token));

			Task workTask() => Task.Run(async () => await DoWork(token));

			var workObservable = Observable.FromAsync(workTask)
										   .DelaySubscription(GetNext().Value);

			//var jobTimes = _expression.GetOccurrences(DateTime.Now, DateTime.Now.AddHours(1));

			if (GetNext(DateTimeOffset.Now).HasValue) {
				var first = GetNext().Value;
				var period = GetNext(first).Value
										   .Subtract(first); //.Subtract(DateTimeOffset.Now);

				var schedule = Observable.Timer(DateTimeOffset.Now, period)
										 .Do(x => Debug.WriteLine($"Running scheduled job at {DateTime.Now.ToShortTimeString()} "))
										 .Select(_ => Observable.StartAsync(workTask))
										 .ObserveOn(scheduler)
										 .SubscribeOn(scheduler)
										 .Subscribe();
				disposer.Add(schedule);

				//Observable.Timer(GetNext().Value)

				//          .ObserveOn(scheduler)
				//          .SubscribeOn(scheduler)
				//          .RepeatWhen()

				//jobTimes.ToObservable()
				//		.

				//Observable.Timer(GetNext(DateTimeOffset.Now).Value)
				//          .Select(GetNext(DateTimeOffset.Now).Value)
				//          .Sub

				//Observable.Timer(GetNext(DateTimeOffset.Now).Value)
				//.Select(_ => GetNext(DateTimeOffset.Now).Value)
			}

			return disposer;
		}

		protected virtual Task ScheduleWithTimer(CancellationToken token) {
			var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
			if (next.HasValue) {
				var delay = next.Value - DateTimeOffset.Now;
				_timer = new System.Timers.Timer(delay.TotalMilliseconds);

				_timer.Elapsed += async (sender, args) => {
					_timer.Stop();  // reset timer
					await DoWork(token);
				};

				_timer.Start();
			}
			return Task.CompletedTask;
		}

	}
}