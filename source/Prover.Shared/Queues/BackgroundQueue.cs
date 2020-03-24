using System;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Shared.Queues
{
    public class BackgroundQueue
    {
        private readonly object _key = new object();
        private Task _previousTask = Task.FromResult(true);

        #region Public Methods

        public Task QueueTask(Action action)
        {
            lock (_key)
            {
                
                _previousTask = _previousTask.ContinueWith(
                    t => action(),
                    CancellationToken.None,
                    TaskContinuationOptions.None,
                    TaskScheduler.Default);
                return _previousTask;
            }
        }

        public Task<T> QueueTask<T>(Func<T> work)
        {
            lock (_key)
            {
                var task = _previousTask.ContinueWith(
                    t => work(),
                    CancellationToken.None,
                    TaskContinuationOptions.None,
                    TaskScheduler.Default);
                _previousTask = task;
                return task;
            }
        }

        #endregion
    }
}