using Devices.Communications.IO;
using Devices.Communications.Messaging;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using NLog;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Communications
{
    public abstract class EvcCommunicationClient<TEvcType> : IDisposable, IEvcCommunicationClient<TEvcType>
        where TEvcType : IEvcDeviceType
    {
        #region Properties

        public virtual TEvcType EvcDeviceType { get; set; }

        /// <summary>
        /// Is this client already connected to an instrument
        /// </summary>
        public abstract bool IsConnected { get; protected set; }

        /// <summary>
        /// Contains all the item numbers and meta data for a specific instrument type
        /// </summary>
        public virtual List<ItemMetadata> ItemDetails { get; protected set; } = new List<ItemMetadata>();

        public IObservable<string> Status { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Establish a link with an instrument Handles retries for failed connections
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <param name="accessCode"></param>
        /// <param name="retryAttempts"></param>
        /// <returns></returns>
        public async Task Connect(CancellationToken ct, int retryAttempts = MaxConnectionAttempts, TimeSpan? timeout = null)
        {
            var connectionAttempts = 1;

            if (IsConnected)
                return;

            if (timeout.HasValue)
                _timeout = timeout.Value;

            ct.ThrowIfCancellationRequested();

            var result = Task.Run(async () =>
            {
                Exception exception = null;

                while (!IsConnected)
                {
                    StatusStream.OnNext($"Connecting to {EvcDeviceType.Name} on {CommPort.Name}... {connectionAttempts} of {MaxConnectionAttempts}");

                    if (ct.IsCancellationRequested)
                        ct.ThrowIfCancellationRequested();

                    if (!CommPort.IsOpen())
                        await CommPort.Open(ct);

                    try
                    {
                        await ConnectToInstrument(ct);
                    }
                    catch (EvcResponseException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }

                    if (!IsConnected)
                    {
                        if (connectionAttempts < retryAttempts)
                        {
                            StatusStream.OnNext($"[{CommPort.Name}] Failed connecting to {EvcDeviceType.Name}.");
                            await Task.Delay(ConnectionRetryDelayMs);
                            connectionAttempts++;
                        }
                        else
                        {
                            throw new FailedConnectionException(CommPort, EvcDeviceType, retryAttempts, exception);
                        }
                    }
                }
            }, ct);

            try
            {
                await result;
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                {
                    Log.Warn(e.Message + " " + v.Message);
                }
                throw;
            }
        }

        /// <summary>
        /// Disconnect the current link with the EVC, if one exists
        /// </summary>
        public abstract Task Disconnect();

        public virtual void Dispose()
        {
            Disconnect();
            StatusStream.OnCompleted();
            _receivedObservable?.Dispose();
            _sentObservable?.Dispose();
            CommPort?.Dispose();
        }

        /// <summary>
        /// Read all items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public virtual Task<IEnumerable<ItemValue>> GetAllItems()
        {
            StatusStream.OnNext("Downloading items...");
            return GetItemValues(EvcDeviceType.Definitions);
        }

        /// <summary>
        /// Read frequency test items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public abstract Task<IFrequencyTestItems> GetFrequencyItems();

        /// <summary>
        /// Read item value from instrument
        /// </summary>
        /// <param name="itemNumber">Item number for the value to request</param>
        /// <returns></returns>
        public abstract Task<ItemValue> GetItemValue(ItemMetadata itemNumber);

        /// <summary>
        /// Read a group of items from instrument
        /// </summary>
        /// <param name="itemNumbers">Item numbers for the values to request</param>
        /// <returns></returns>
        public abstract Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers);

        /// <summary>
        /// Read pressure test items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public virtual Task<IEnumerable<ItemValue>> GetPressureTestItems()
        {
            return GetItemValues(EvcDeviceType.Definitions.PressureItems());
        }

        /// <summary>
        /// Read temperature test items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public virtual Task<IEnumerable<ItemValue>> GetPulseOutputItems()
        {
            return GetItemValues(ItemDetails.PulseOutputItems());
        }

        /// <summary>
        /// Read temperature test items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public virtual Task<IEnumerable<ItemValue>> GetTemperatureTestItems()
        {
            return GetItemValues(EvcDeviceType.Definitions.TemperatureItems());
        }

        ///// <summary>
        /////     Read a group of items from instrument
        ///// </summary>
        ///// <param name="itemNumbers">Item numbers for the values to request</param>
        ///// <returns></returns>
        //public abstract Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<int> itemNumbers);
        /// <summary>
        /// Read volume items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public virtual Task<IEnumerable<ItemValue>> GetVolumeItems()
        {
            return GetItemValues(EvcDeviceType.Definitions.VolumeItems());
        }

        /// <summary>
        /// Live read item values Gas Temp / Gas Pressure
        /// </summary>
        /// <param name="itemNumber">Item number to live read</param>
        /// <returns></returns>
        public abstract Task<ItemValue> LiveReadItemValue(int itemNumber);

        /// <summary>
        /// Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        public abstract Task<bool> SetItemValue(int itemNumber, string value);

        /// <summary>
        /// Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        public abstract Task<bool> SetItemValue(int itemNumber, decimal value);

        /// <summary>
        /// Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        public abstract Task<bool> SetItemValue(int itemNumber, long value);

        /// <summary>
        /// Write a value to an item
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract Task<bool> SetItemValue(string itemCode, long value);

        #endregion

        #region Fields

        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        /// <summary>
        /// A client to communicate with a wide range of EVCs
        /// </summary>
        /// <param name="commPort">Communcations interface to the device</param>
        /// <param name="EvcDeviceType">Instrument type of device</param>
        /// <param name="statusSubject">Subject for listening to status updates</param>
        protected EvcCommunicationClient(ICommPort commPort, TEvcType evcDeviceType)
        {
            CommPort = commPort;
            EvcDeviceType = evcDeviceType;

            Status = StatusStream.Publish();
            var statusDisposable = (Status as IConnectableObservable<string>).Connect();

            Status.Subscribe(
                s => Log.Debug(s),
                () => statusDisposable.Dispose());

            _receivedObservable = ResponseProcessors.MessageProcessor.ResponseObservable(CommPort.DataReceivedObservable)
                .Subscribe(msg => { Log.Debug($"[{CommPort.Name}] [R] {ControlCharacters.Prettify(msg)}"); });

            _sentObservable = CommPort.DataSentObservable
                .Subscribe(msg => { Log.Debug($"[{CommPort.Name}] [S] {ControlCharacters.Prettify(msg)}"); });
        }

        #endregion

        public ICommPort CommPort { get; }

        protected readonly ISubject<string> StatusStream = new Subject<string>();

        /// <summary>
        /// Establish a link with an instrument Handles retries for failed connections
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="accessCode"></param>
        /// <returns></returns>
        protected abstract Task ConnectToInstrument(CancellationToken ct);

        protected virtual void ExecuteCommand(string command)
        {
            CommPort.Send(command);
        }

        protected virtual async Task<T> ExecuteCommand<T>(CommandDefinition<T> command) where T : ResponseMessage
        {
            if (command.ResponseProcessor == null)
            {
                ExecuteCommand(command.Command);
                return default(T);
            }

            var response = command.ResponseProcessor.ResponseObservable(CommPort.DataReceivedObservable)
                .Timeout(_timeout)
                .FirstAsync()
                .PublishLast();

            using (response.Connect())
            {
                ExecuteCommand(command.Command);

                return await response;
            }
        }

        private const int ConnectionRetryDelayMs = 3000;

        private const int MaxConnectionAttempts = 10;

        private readonly IDisposable _receivedObservable;

        private readonly IDisposable _sentObservable;

        private TimeSpan _timeout = TimeSpan.FromSeconds(5);
    }
}