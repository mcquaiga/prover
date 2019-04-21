namespace Module.EvcVerification.VerificationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;
    using LogManager = NLog.LogManager;

    #region Interfaces

    /// <summary>
    /// Defines the <see cref="IReadingStabilizer" />
    /// </summary>
    public interface IReadingStabilizer
    {
        #region Methods

        /// <summary>
        /// The WaitForReadingsToStabilizeAsync
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <param name="level">The level<see cref="int"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <param name="statusUpdates">The statusUpdates<see cref="Subject{string}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task WaitForReadingsToStabilizeAsync(EvcCommunicationClient commClient, Instrument instrument, int level, CancellationToken ct, Subject<string> statusUpdates);

        #endregion
    }

    #endregion

    /// <summary>
    /// Defines the <see cref="AverageReadingStabilizer" />
    /// </summary>
    public class AverageReadingStabilizer : IReadingStabilizer
    {
        #region Fields

        /// <summary>
        /// Defines the _log
        /// </summary>
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AverageReadingStabilizer"/> class.
        /// </summary>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        public AverageReadingStabilizer(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the EventAggregator
        /// </summary>
        public IEventAggregator EventAggregator { get; set; }

        public ReplaySubject<LiveReadEvent> LiveReadStatusUpdates = new ReplaySubject<LiveReadEvent>();
        #endregion

        #region Methods

        /// <summary>
        /// The WaitForReadingsToStabilizeAsync
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <param name="level">The level<see cref="int"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <param name="statusUpdates">The statusUpdates<see cref="Subject{string}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task WaitForReadingsToStabilizeAsync(EvcCommunicationClient commClient, Instrument instrument, int level, CancellationToken ct, Subject<string> statusUpdates)
        {
            try
            {            
                var liveReadItems = GetLiveReadItemNumbers(instrument, level);
                
                await commClient.Connect(ct);
                
                ct.ThrowIfCancellationRequested();
                do
                {
                    string status = string.Empty;
                    foreach (var item in liveReadItems)
                    {
                        var liveValue = await commClient.LiveReadItemValue(item.Key.Metadata.Number);
                        item.Value.Add(liveValue.NumericValue);                       
                    }
                    this.Publish(new LiveReadStatusEvent($"Stabilizing live readings...", liveReadItems));   

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

        /// <summary>
        /// The GetLiveReadItemNumbers
        /// </summary>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <param name="level">The level<see cref="int"/></param>
        /// <returns>The <see cref="Dictionary{ItemValue, AveragedReadingStabilizer}"/></returns>
        private Dictionary<ItemValue, AveragedReadingStabilizer> GetLiveReadItemNumbers(Instrument instrument, int level)
        {
            var test = instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level);
            var pressure = test?.PressureTest?.GasGauge ?? 0m;
            var temperature = (double?)test?.TemperatureTest?.Gauge ?? 0m;

            var liveReadItems = new Dictionary<ItemValue, AveragedReadingStabilizer>();

            if (instrument.CompositionType == EvcCorrectorType.T || instrument.CompositionType == EvcCorrectorType.PTZ)
                liveReadItems.Add(instrument.Items.First(x => x.Metadata.IsLiveReadTemperature ?? false), new AveragedReadingStabilizer(temperature));

            if (instrument.CompositionType == EvcCorrectorType.P || instrument.CompositionType == EvcCorrectorType.PTZ)
                liveReadItems.Add(instrument.Items.First(x => x.Metadata.IsLiveReadPressure ?? false), new AveragedReadingStabilizer(pressure));
            return liveReadItems;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="AveragedReadingStabilizer" />
    /// </summary>
    public class AveragedReadingStabilizer
    {
        #region Constants

        /// <summary>
        /// Defines the AverageThreshold
        /// </summary>
        private const double AverageThreshold = 1.0m;

        /// <summary>
        /// Defines the FixedQueueSize
        /// </summary>
        private const int FixedQueueSize = 20;

        #endregion

        #region Fields

        /// <summary>
        /// Defines the _valueQueue
        /// </summary>
        private FixedSizedQueue<double> _valueQueue;
        private double _latest;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AveragedReadingStabilizer"/> class.
        /// </summary>
        /// <param name="gaugeValue">The gaugeValue<see cref="double"/></param>
        public AveragedReadingStabilizer(double gaugeValue)
        {
            GaugeValue = gaugeValue;
            _valueQueue = new FixedSizedQueue<double>(FixedQueueSize);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the GaugeValue
        /// </summary>
        public double GaugeValue { get; }

        /// <summary>
        /// Gets a value indicating whether IsStable
        /// </summary>
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

        #endregion

        #region Methods

        /// <summary>
        /// The Add
        /// </summary>
        /// <param name="value">The value<see cref="double"/></param>
        public void Add(double value)
        {
            _valueQueue.Enqueue(value);
            _latest = value;
        }

        /// <summary>
        /// The Clear
        /// </summary>
        public void Clear()
        {
            _valueQueue = null;
            _valueQueue = new FixedSizedQueue<double>(FixedQueueSize);
        }

        public double Latest()
        {
            return _latest;
        }

        #endregion
    }
}
