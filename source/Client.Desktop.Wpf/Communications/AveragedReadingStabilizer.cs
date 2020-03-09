using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.Desktop.Wpf.Communications
{
    /// <summary>
    ///     Defines the <see cref="AveragedReadingStabilizer" />
    /// </summary>
    public class AveragedReadingStabilizer
    {
        /// <summary>
        ///     Defines the AverageThreshold
        /// </summary>
        private readonly decimal _averageThreshold = 1.0m;

        /// <summary>
        ///     Defines the FixedQueueSize
        /// </summary>
        private readonly int _fixedQueueSize = 20;

        /// <summary>
        ///     Defines the _valueQueue
        /// </summary>
        private readonly Queue<decimal> _valueQueue;

        private decimal _latest;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AveragedReadingStabilizer" /> class.
        /// </summary>
        /// <param name="target">The gaugeValue<see cref="decimal" /></param>
        public AveragedReadingStabilizer(decimal target, int? queueSize = null, decimal? threshold = null)
        {
            _fixedQueueSize = queueSize ?? _fixedQueueSize;
            _averageThreshold = threshold ?? _averageThreshold;
            TargetValue = target;
            _valueQueue = new Queue<decimal>(_fixedQueueSize);
        }

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
            if (_valueQueue.Count == _fixedQueueSize) _valueQueue.Dequeue();

            _valueQueue.Enqueue(value);
            _latest = value;
        }

        /// <summary>
        ///     Gets a value indicating whether IsStable
        /// </summary>
        public bool CheckIfStable()
        {
            if (_valueQueue.Count >= _fixedQueueSize)
            {
                var average = _valueQueue.Sum() / _valueQueue.Count;
                var difference = Math.Abs(TargetValue - average);

                if (difference <= _averageThreshold)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     The Clear
        /// </summary>
        public void Clear()
        {
            _valueQueue.Clear();
        }

        public decimal Latest() => _latest;
    }
}