using System.Reactive.Concurrency;

namespace Prover.Shared.Interfaces
{
    public interface ISchedulerProvider
    {
        IScheduler CurrentThread { get; }
        IScheduler Dispatcher { get; }
        IScheduler Immediate { get; }
        IScheduler NewThread { get; }

        IScheduler ThreadPool { get; }
        IScheduler TaskPool { get; } 
    }
}