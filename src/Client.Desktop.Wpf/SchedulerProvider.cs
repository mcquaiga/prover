using System.Reactive.Concurrency;
using Prover.Application.Interfaces;
using Prover.Shared.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf
{
    public sealed class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler CurrentThread => Scheduler.CurrentThread;

        public IScheduler Dispatcher => RxApp.MainThreadScheduler;

        public IScheduler Immediate => Scheduler.Immediate;

        public IScheduler NewThread => NewThreadScheduler.Default;

        public IScheduler ThreadPool => ThreadPoolScheduler.Instance;

        public IScheduler TaskPool => TaskPoolScheduler.Default;
    }
}