using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using NLog;
using Prover.CommProtocol.Common;
using Prover.Core.Collections;
using Prover.Core.Events;
using Prover.Core.Models.Instruments;
using Prover.Core.Shared.Enums;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests
{
    public interface IReadingStabilizer
    {
        Task WaitForReadingsToStabilizeAsync(EvcCommunicationClient commClient, Instrument instrument, int level,
            CancellationToken ct);
    }

    public class AverageReadingStabilizer : IReadingStabilizer
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public AverageReadingStabilizer(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        public IEventAggregator EventAggregator { get; set; }

        public async Task WaitForReadingsToStabilizeAsync(EvcCommunicationClient commClient, Instrument instrument,
            int level, CancellationToken ct)
        {
            try
            {
                var liveReadItems = GetLiveReadItemNumbers(instrument, level);
                await commClient.Connect(ct);
                ct.ThrowIfCancellationRequested();
                do
                {
                    foreach (var item in liveReadItems)
                    {
                        var liveValue = await commClient.LiveReadItemValue(item.Key);
                        item.Value.Add(liveValue.NumericValue);
                        EventAggregator.PublishOnBackgroundThread(new LiveReadEvent(item.Key, liveValue.NumericValue));
                    }

                    ct.ThrowIfCancellationRequested();
                } while (liveReadItems.Any(x => !x.Value.IsStable));
            }
            catch (OperationCanceledException)
            {
                _log.Info("Cancelled live reading.");
                throw;
            }
            finally
            {
                if (commClient != null)
                    await commClient.Disconnect();
            }
        }

        private Dictionary<int, AveragedReadingStabilizer> GetLiveReadItemNumbers(Instrument instrument, int level)
        {
            var test = instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level);
            var pressure = test?.PressureTest?.GasGauge ?? 0m;
            var temperature = (decimal?) test?.TemperatureTest?.Gauge ?? 0m;

            var liveReadItems = new Dictionary<int, AveragedReadingStabilizer>();

            if (instrument.CompositionType == EvcCorrectorType.T || instrument.CompositionType == EvcCorrectorType.PTZ)
                liveReadItems.Add(26, new AveragedReadingStabilizer(temperature));

            if (instrument.CompositionType == EvcCorrectorType.P || instrument.CompositionType == EvcCorrectorType.PTZ)
                liveReadItems.Add(8, new AveragedReadingStabilizer(pressure));
            return liveReadItems;
        }
    }

    internal class AveragedReadingStabilizer
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
                    var average = _valueQueue.Sum() / FixedQueueSize;
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