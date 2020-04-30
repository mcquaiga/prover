using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using Prover.Shared.Interfaces;

namespace Tests.Shared
{
    public sealed class TestSchedulers : ISchedulerProvider
    {
        IScheduler ISchedulerProvider.CurrentThread => CurrentThread;
        IScheduler ISchedulerProvider.Dispatcher => Dispatcher;
        IScheduler ISchedulerProvider.Immediate => Immediate;
        IScheduler ISchedulerProvider.NewThread => NewThread;
        IScheduler ISchedulerProvider.ThreadPool => ThreadPool;
        IScheduler ISchedulerProvider.TaskPool => TaskPool;
        public TestScheduler CurrentThread { get; } = new TestScheduler();

        public TestScheduler Dispatcher { get; } = new TestScheduler();

        public TestScheduler Immediate { get; } = new TestScheduler();

        public TestScheduler NewThread { get; } = new TestScheduler();

        public TestScheduler ThreadPool { get; } = new TestScheduler();

        public TestScheduler TaskPool { get; } = new TestScheduler();
    }

    public sealed class ImmediateSchedulers : ISchedulerProvider
    {
        public IScheduler CurrentThread => Scheduler.Immediate;
        public IScheduler Dispatcher => Scheduler.Immediate;
        public IScheduler Immediate => Scheduler.Immediate;
        public IScheduler NewThread => Scheduler.Immediate;
        public IScheduler ThreadPool => Scheduler.Immediate;
        public IScheduler TaskPool => TaskPoolScheduler.Default;
    }
}