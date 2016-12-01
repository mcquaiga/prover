using System.Collections.Concurrent;

namespace Prover.Core.Collections
{
    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        private readonly object _syncObject = new object();

        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public int Size { get; }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (_syncObject)
            {
                while (Count > Size)
                {
                    T outObj;
                    TryDequeue(out outObj);
                }
            }
        }
    }
}