using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Communications.Messaging;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Communications
{
    public static class CommClient
    {
        public static Task<IEvcCommunicationClient<TDevice>> CreateAsync<TDevice>(this TDevice deviceType, ICommPort commPort, int retryAttempts = 10, TimeSpan? timeout = null)
            where TDevice : IDeviceType
        {
            var types = GetAllTypesImplementingOpenGenericType(typeof(ICommClientFactory<>), AppDomain.CurrentDomain.GetAssemblies());

            var factory = types.FirstOrDefault();

            var instance = (Activator.CreateInstance(factory) as ICommClientFactory<TDevice>);

            return instance.Create(deviceType, commPort, retryAttempts, timeout);
        }

        private static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly[] assemblies)
        {
            return from ass in assemblies
                   from x in ass.GetTypes()
                   from z in x.GetInterfaces()
                   let y = x.BaseType
                   where
                   (y != null && y.IsGenericType &&
                   openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
                   (z.IsGenericType &&
                   openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                   select x;
        }
    }

    public abstract class EvcCommunicationClient<T> : IDisposable, IEvcCommunicationClient<T>
        where T : IDeviceType
    {
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();
        protected readonly ISubject<string> StatusStream = new Subject<string>();
        private const int ConnectionRetryDelayMs = 3000;
        private const int MaxConnectionAttempts = 10;
        private readonly IDisposable _receivedObservable;
        private readonly IDisposable _sentObservable;
        private TimeSpan _timeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// A client to communicate with a wide range of EVCs
        /// </summary>
        /// <param name="commPort">Communcations interface to the device</param>
        /// <param name="EvcDeviceType">Instrument type of device</param>
        /// <param name="statusSubject">Subject for listening to status updates</param>
        protected EvcCommunicationClient(ICommPort commPort)
        {
            CommPort = commPort;
            CreateStreams();
        }

        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public ICommPort CommPort { get; }
        public virtual T DeviceType { get; protected set; }

        /// <summary>
        /// Is this client already connected to an instrument
        /// </summary>
        public abstract bool IsConnected { get; protected set; }

        /// <summary>
        /// Contains all the item numbers and meta data for a specific instrument type
        /// </summary>
        public virtual List<ItemMetadata> ItemDetails { get; protected set; } = new List<ItemMetadata>();

        public IObservable<string> Status { get; private set; }
        protected virtual CancellationToken CancellationToken => CancellationTokenSource.Token;

        public Task Cancel()
        {
            return Task.Run(() => CancellationTokenSource?.Cancel());
        }

        public Task ConnectAsync(int retryAttempts = MaxConnectionAttempts, TimeSpan? timeout = null)
        {
            return ConnectAsync(DeviceType, retryAttempts, timeout);
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
        ///
        public virtual Task<IEnumerable<ItemValue>> GetAllItems()
        {
            StatusStream.OnNext("Downloading items...");
            return GetItemValues(DeviceType.Definitions);
        }

        public async Task<IDevice> GetDevice()
        {
            var values = await GetAllItems();
            return DeviceType.CreateInstance(values);
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
            return GetItemValues(DeviceType.Definitions.PressureItems());
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
            return GetItemValues(DeviceType.Definitions.TemperatureItems());
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
            return GetItemValues(DeviceType.Definitions.VolumeItems());
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

        /// <summary>
        /// Establish a link with an instrument Handles retries for failed connections
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <param name="accessCode"></param>
        /// <param name="retryAttempts"></param>
        /// <returns></returns>
        protected virtual async Task ConnectAsync(IDeviceType deviceType, int retryAttempts = MaxConnectionAttempts, TimeSpan? timeout = null)
        {
            CancellationTokenSource = new CancellationTokenSource();

            if (deviceType == null)
            {
                throw new NullReferenceException("Device type is not initialized. Call this method with the device type first.");
            }

            var connectionAttempts = 1;

            if (IsConnected)
                return;

            if (timeout.HasValue)
                _timeout = timeout.Value;

            CancellationToken.ThrowIfCancellationRequested();

            Exception exception = null;

            while (!IsConnected || connectionAttempts <= retryAttempts)
            {
                StatusStream.OnNext($"Connecting to {deviceType.Name} on {CommPort.Name}... {connectionAttempts} of {MaxConnectionAttempts}");

                if (CancellationToken.IsCancellationRequested)
                    CancellationToken.ThrowIfCancellationRequested();

                if (!CommPort.IsOpen())
                    await CommPort.Open(CancellationToken);

                try
                {
                    await EstablishConnectionAsync(DeviceType, CancellationToken);
                }
                catch (EvcResponseException eex)
                {
                    throw eex;
                }
                catch (Exception ex)
                {
                    StatusStream.OnNext($"[{CommPort.Name}] Failed connecting to {deviceType.Name}.");
                    await Task.Delay(ConnectionRetryDelayMs);
                    connectionAttempts++;
                    exception = ex;
                }
            }

            if (!IsConnected)
            {
                throw new FailedConnectionException(CommPort, deviceType, retryAttempts, exception);
            }
        }

        /// <summary>
        /// Establish a link with an instrument Handles retries for failed connections
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="accessCode"></param>
        /// <returns></returns>
        protected abstract Task EstablishConnectionAsync(IDeviceType deviceType, CancellationToken ct);

        protected virtual void ExecuteCommand(string command)
        {
            CommPort.Send(command);
        }

        protected virtual async Task<T> ExecuteCommandAsync<T>(CommandDefinition<T> command) where T : ResponseMessage
        {
            if (command.ResponseProcessor == null)
            {
                ExecuteCommand(command.Command);
                return default;
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

        private void CreateStreams()
        {
            var incoming = ResponseProcessors.MessageProcessor.ResponseObservable(CommPort.DataReceivedObservable)
                            .Select(msg => $"[{CommPort.Name}] [R] {ControlCharacters.Prettify(msg)}")
                            .Publish();

            var outgoing = CommPort.DataSentObservable
                .Select(msg => $"[{CommPort.Name}] [S] {ControlCharacters.Prettify(msg)}")
                .Publish();

            var chatStream = incoming
                .Merge(outgoing)
                .Publish();

            chatStream
                .Subscribe(msg =>
                {
                    Log.Debug(msg);
                    Console.WriteLine(msg);
                });

            Status = StatusStream.Publish();
            var statusDisposable = (Status as IConnectableObservable<string>).Connect();

            incoming.Connect();
            outgoing.Connect();
            chatStream.Connect();

            Status.Subscribe(
                s => Console.WriteLine(s),
                () => statusDisposable.Dispose());
        }
    }
}