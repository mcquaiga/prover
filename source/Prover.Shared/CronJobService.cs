using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;

namespace Prover.UI.Desktop.Common {
	public class CronSchedules {
		public static CronExpression Daily { get; } = CronExpression.Parse("0 0 * * *");
		public static CronExpression Hourly { get; } = CronExpression.Parse("0 * * * *");
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
			if (_expression != null)
				await ScheduleJob(cancellationToken);
		}

		protected virtual async Task ScheduleJob(CancellationToken cancellationToken) {
			var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
			if (next.HasValue) {
				var delay = next.Value - DateTimeOffset.Now;
				_timer = new System.Timers.Timer(delay.TotalMilliseconds);
				_timer.Elapsed += async (sender, args) => {
					_timer.Stop();  // reset timer
					await DoWork(cancellationToken);
					await ScheduleJob(cancellationToken);    // reschedule next
				};
				_timer.Start();
			}
			await Task.CompletedTask;
		}

		public abstract Task DoWork(CancellationToken cancellationToken);

		public virtual async Task StopAsync(CancellationToken cancellationToken) {
			_timer?.Stop();
			await Task.CompletedTask;
		}

		public virtual void Dispose() {
			_timer?.Dispose();
		}
	}
}