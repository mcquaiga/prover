using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Devices.Communications.Interfaces;
using Devices.Core.Items;

namespace Prover.Application.ViewModels.Corrections
{
    /// <summary>
    ///     Defines the <see cref="IReadingStabilizer" />
    /// </summary>
    public interface IReadingStabilizer
    {
        /// <summary>
        ///     The WaitForReadingsToStabilizeAsync
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient" /></param>
        /// <param name="instrument">The instrument<see cref="Instrument" /></param>
        /// <param name="level">The level<see cref="int" /></param>
        /// <param name="ct">The ct<see cref="CancellationToken" /></param>
        /// <param name="statusUpdates">The statusUpdates<see cref="Subject" /></param>
        /// <returns>The <see cref="Task" /></returns>
        Task WaitForReadingsToStabilizeAsync(ICommunicationsClient commClient,
            ICollection<LiveReadItemTarget> itemTargets,
            CancellationToken ct, IObserver<ItemValue> statusUpdates);
    }

    public class LiveReadItemTarget
    {
        public ItemMetadata Item { get; set; }
        public decimal Target { get; set; }


        // //public ICollection<>
        //public class ItemTargets
        //{

        //}
    }

    /// <summary>
    ///     Defines the <see cref="AverageReadingStabilizer" />
    /// </summary>
    //public class AverageReadingStabilizer : IReadingStabilizer
    //{
    //    /// <summary>
    //    ///     Defines the _log
    //    /// </summary>
    //    private readonly Logger<AverageReadingStabilizer> _log;

    //    /// <summary>
    //    ///     The WaitForReadingsToStabilizeAsync
    //    /// </summary>
    //    /// <param name="commClient">The commClient<see cref="ICommunicationsClient" /></param>
    //    /// <param name="itemTargets"></param>
    //    /// <param name="ct">The ct<see cref="CancellationToken" /></param>
    //    /// <param name="statusUpdates">The statusUpdates<see /></param>
    //    /// <returns>The <see cref="Task" /></returns>
    //    public async Task WaitForReadingsToStabilizeAsync(ICommunicationsClient commClient, ICollection<LiveReadItemTarget> itemTargets,
    //        CancellationToken ct, IObserver<ItemValue> statusUpdates)
    //    {
    //        try
    //        {
    //            await commClient.Connect(ct);

    //            ct.ThrowIfCancellationRequested();
    //            do
    //            {
    //                var status = string.Empty;
    //                foreach (var item in liveReadItems)
    //                {
    //                    var liveValue = await commClient.LiveReadItemValue(item.Key.Metadata.Number);
    //                    item.Value.Add(liveValue.NumericValue);
    //                }

    //                this.Publish(new LiveReadStatusEvent("Stabilizing live readings...", liveReadItems));

    //                ct.ThrowIfCancellationRequested();
    //            } while (liveReadItems.Any(x => !x.Value.IsStable));
    //        }
    //        catch (OperationCanceledException)
    //        {
    //            _log.LogInformation("Cancelled live reading.");
    //            throw;
    //        }
    //        finally
    //        {
    //            if (commClient != null)
    //                await commClient.Disconnect();
    //        }
    //    }

    //    /// <summary>
    //    ///     The GetLiveReadItemNumbers
    //    /// </summary>
    //    /// <param name="instrument">The instrument<see cref="Instrument" /></param>
    //    /// <param name="level">The level<see cref="int" /></param>
    //    /// <returns>The <see cref="Dictionary{ItemValue, AveragedReadingStabilizer}" /></returns>
    //    private Dictionary<ItemValue, AveragedReadingStabilizer> GetLiveReadItemNumbers(Instrument instrument,
    //        int level)
    //    {
    //        var test = instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level);
    //        var pressure = test?.PressureTest?.GasGauge ?? 0m;
    //        var temperature = (decimal?) test?.TemperatureTest?.Gauge ?? 0m;

    //        var liveReadItems = new Dictionary<ItemValue, AveragedReadingStabilizer>();

    //        if (instrument.CompositionType == EvcCorrectorType.T || instrument.CompositionType == EvcCorrectorType.PTZ)
    //            liveReadItems.Add(instrument.Items.First(x => x.Metadata.IsLiveReadTemperature ?? false),
    //                new AveragedReadingStabilizer(temperature));

    //        if (instrument.CompositionType == EvcCorrectorType.P || instrument.CompositionType == EvcCorrectorType.PTZ)
    //            liveReadItems.Add(instrument.Items.First(x => x.Metadata.IsLiveReadPressure ?? false),
    //                new AveragedReadingStabilizer(pressure));
    //        return liveReadItems;
    //    }

    //    //public ReplaySubject<LiveReadEvent> LiveReadStatusUpdates = new ReplaySubject<LiveReadEvent>();
    //}


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

        private decimal _latest;

        /// <summary>
        ///     Defines the _valueQueue
        /// </summary>
        private Queue<decimal> _valueQueue;

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
            _valueQueue.Enqueue(value);
            _latest = value;
        }

        /// <summary>
        ///     Gets a value indicating whether IsStable
        /// </summary>
        public bool CheckIfStable()
        {
            if (_valueQueue.Count == _fixedQueueSize)
            {
                var average = _valueQueue.Sum() / _fixedQueueSize;
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
            _valueQueue = null;
            _valueQueue = new Queue<decimal>(_fixedQueueSize);
        }

        public decimal Latest() => _latest;
    }
}