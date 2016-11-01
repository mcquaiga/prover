using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.Core.Collections;
using Prover.Core.Events;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests
{
    public interface IReadingStabilizer
    {
        Task WaitForReadingsToStabilizeAsync(EvcCommunicationClient commClient, Instrument instrument, int level);
        void CancelLiveReading();
    }

    public class AverageReadingStabilizer : IReadingStabilizer
    {
        private bool _cancelStabilize;
        private bool _isLiveReading;

        public AverageReadingStabilizer(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        public IEventAggregator EventAggregator { get; set; }

        public async Task WaitForReadingsToStabilizeAsync(EvcCommunicationClient commClient, Instrument instrument,
            int level)
        {
            var liveReadItems = GetLiveReadItemNumbers(instrument, level);

            await commClient.Connect();

            do
            {
                _isLiveReading = true;
                foreach (var item in liveReadItems)
                {
                    var liveValue = await commClient.LiveReadItemValue(item.Key);
                    item.Value.Add(liveValue.NumericValue);
                    EventAggregator.PublishOnBackgroundThread(new LiveReadEvent(item.Key, liveValue.NumericValue));
                }
            } while (liveReadItems.Any(x => !x.Value.IsStable) && !_cancelStabilize);

            _isLiveReading = false;
            _cancelStabilize = false;

            await commClient.Disconnect();
        }

        public void CancelLiveReading()
        {
            if (_isLiveReading)
                _cancelStabilize = true;
        }

        private Dictionary<int, AveragedReadingStabilizer> GetLiveReadItemNumbers(Instrument instrument, int level)
        {
            var liveReadItems = new Dictionary<int, AveragedReadingStabilizer>();
            if (instrument.CompositionType == CorrectorType.PTZ)
            {
                liveReadItems.Add(8, new AveragedReadingStabilizer(instrument.GetGaugePressure(level)));
                liveReadItems.Add(26, new AveragedReadingStabilizer(instrument.GetGaugeTemp(level)));
            }

            if (instrument.CompositionType == CorrectorType.T)
                liveReadItems.Add(26, new AveragedReadingStabilizer(instrument.GetGaugeTemp(level)));

            if (instrument.CompositionType == CorrectorType.P)
                liveReadItems.Add(8, new AveragedReadingStabilizer(instrument.GetGaugePressure(level)));
            return liveReadItems;
        }
    }

    public class AveragedReadingStabilizer
    {
        private const decimal AverageThreshold = 1.0m;
        private const int FixedQueueSize = 20;
        private FixedSizedQueue<decimal> _valueQueue;

        public AveragedReadingStabilizer(decimal gaugeValue)
        {
            GaugeValue = gaugeValue;
            _valueQueue = new FixedSizedQueue<decimal>(FixedQueueSize);
        }

        public decimal GaugeValue { get; }

        public bool IsStable
        {
            get
            {
                if (_valueQueue.Count == FixedQueueSize)
                {
                    var average = _valueQueue.Sum()/FixedQueueSize;
                    var difference = Math.Abs(GaugeValue - average);

                    if (difference <= AverageThreshold)
                        return true;
                }

                return false;
            }
        }

        public void Add(decimal value)
        {
            _valueQueue.Enqueue(value);
        }

        public void Clear()
        {
            _valueQueue = null;
            _valueQueue = new FixedSizedQueue<decimal>(FixedQueueSize);
        }
    }
}