using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Subjects;

namespace Prover.Application.Services.LiveReadCorrections
{
    /// <summary>
    ///     Defines the <see cref="AveragedReadingStabilizer" />
    /// </summary>
    public class AveragedReadingStabilizer
    {
        /// <summary>
        ///     Defines the AverageThreshold
        /// </summary>
        private readonly decimal _differenceThreshold = 1.0m;

        /// <summary>
        ///     Defines the FixedQueueSize
        /// </summary>
        private readonly int _fixedQueueSize = 20;

        /// <summary>
        ///     Defines the _valueQueue
        /// </summary>
        private readonly ConcurrentQueue<decimal> _valueQueue = new ConcurrentQueue<decimal>();

        private decimal _latest;
        private Subject<bool> _stableSubject = new Subject<bool>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="AveragedReadingStabilizer" /> class.
        /// </summary>
        /// <param name="target">The gaugeValue<see cref="decimal" /></param>
        public AveragedReadingStabilizer(decimal target, int? queueSize = null, decimal? threshold = null)
        {
            _fixedQueueSize = queueSize ?? _fixedQueueSize;
            _differenceThreshold = threshold ?? _differenceThreshold;
            TargetValue = target;
        }

        public decimal TargetThreshold => _differenceThreshold;
        /// <summary>
        ///     Gets the GaugeValue
        /// </summary>
        public decimal TargetValue { get; }

        /// <summary>
        ///     The Add
        /// </summary>
        /// <param name="value">The value<see cref="decimal" /></param>
        public void Add(decimal value)
        {
            if (_valueQueue.Count == _fixedQueueSize)
            {
                _valueQueue.TryDequeue(out var removed);

            }
            _valueQueue.Enqueue(value);
            _latest = value;
            
            IsStable = CheckIfStable();
        }

        public decimal Difference => (Average - TargetValue).Round(2);

        /// <summary>
        ///     Gets a value indicating whether IsStable
        /// </summary>
        public bool CheckIfStable()
        {
            if (_valueQueue.Count >= _fixedQueueSize)
            {
                return Math.Abs(Difference) <= _differenceThreshold;
            }

            return false;
        }

        public bool IsStable { get; private set; }

        public decimal Average => (_valueQueue.Sum() / _valueQueue.Count).Round(2);
        
        public decimal Progress()
        {
            var x = _valueQueue.Count / _fixedQueueSize;
            return TargetThreshold - (Math.Abs(Difference) / (TargetThreshold * 10));
        }

        public IObservable<bool> StableObservable => _stableSubject;
        
        public decimal Latest() => _latest;
    }
}