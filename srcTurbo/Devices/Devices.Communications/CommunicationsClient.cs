using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Communications.Messaging;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;

namespace Devices.Communications
{
    public abstract class CommunicationsClient<TDevice, TInstance> : ICommunicationsClient<TDevice, TInstance>, IDisposable
        where TDevice : DeviceType
        where TInstance : DeviceInstance
    {
        private const int ConnectionRetryDelayMs = 3000;
        private const int MaxConnectionAttempts = 10;

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(3);

        protected readonly ISubject<string> StatusObservable = new Subject<string>();
        private IDisposable _messagingConnection;
        private IConnectableObservable<string> _messagingStream;
        private IConnectableObservable<string> _statusConnectableObservable;
        private IDisposable _statusConnection;

        /// <summary>
        ///     A client to communicate with a wide range of EVCs
        /// </summary>
        /// <param name="commPort">Communcations interface to the device</param>
        /// <param name="deviceType"></param>
        /// <param name="EvcDeviceType">Instrument type of device</param>
        /// <param name="statusSubject">Subject for listening to status updates</param>
        protected CommunicationsClient(ICommPort commPort, TDevice deviceType)
        {
            CommPort = commPort;
            DeviceType = deviceType;
            SetupStreams();
        }

        #region Public

        #region Properties

        public virtual TInstance DeviceInstance { get; protected set; }

        /// <summary>
        ///     Is this client already connected to an instrument
        /// </summary>
        public abstract bool IsConnected { get; protected set; }

        public ICommPort CommPort { get; }

        public IObservable<string> CommunicationMessages => _messagingStream;

        public TDevice DeviceType { get; }

        public IObservable<string> StatusMessages => _statusConnectableObservable;

        #endregion

        #region Methods

        public void Cancel()
        {
            try
            {
                CancellationTokenSource.Cancel(false);
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    Console.WriteLine($"Exception {ex.Message}");
                }
            }
            
        }

        public Task ConnectAsync(int retryAttempts = MaxConnectionAttempts, TimeSpan? timeout = null)
        {
            return TryConnectAsync(retryAttempts, timeout);
        }

        public async Task<TInstance> GetDeviceAsync()
        {
            return await GetDeviceAsync(DeviceType);
        }

        public virtual async Task<TInstance> GetDeviceAsync(TDevice deviceType)
        {
            var values = await GetItemsAsync();
            DeviceInstance = (TInstance) deviceType.CreateInstance(values);
            return DeviceInstance;
        }

        public Task<IEnumerable<ItemValue>> GetItemsAsync(IEnumerable<ItemMetadata> itemNumbers)
        {
            PublishStatusMessage("Downloading items...");
            return GetItemValuesAsync(itemNumbers);
        }

        public Task<IEnumerable<ItemValue>> GetItemsAsync()
        {
            PublishStatusMessage("Downloading items...");
            return GetItemValuesAsync(DeviceType.Items);
        }

        public async Task<T> GetItemsAsync<T>()
            where T : IItemGroup
        {
            var items = DeviceType.GetItemsByGroup<T>();
            var values = await GetItemValuesAsync(items);
            return DeviceInstance.GetItemsByGroup<T>(values);
        }

        public virtual void Dispose()
        {
            Disconnect();
            StatusObservable.OnCompleted();
            _statusConnection.Dispose();
            _messagingConnection.Dispose();
            CommPort?.Dispose();
        }

        /// <summary>
        ///     Disconnect the current link with the EVC, if one exists
        /// </summary>
        public abstract Task Disconnect();

        #endregion

        #endregion

        #region Protected

        protected virtual CancellationToken CancellationToken => CancellationTokenSource.Token;

        protected virtual CancellationTokenSource CancellationTokenSource { get; private set; } =
            new CancellationTokenSource();

        //protected abstract Task EstablishConnectionAsync<T>(T deviceType, CancellationToken ct) where T : IDeviceType;
        protected abstract Task EstablishConnectionAsync(CancellationToken ct);

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

            var response = command.ResponseProcessor.ResponseObservable(CommPort.DataReceived)
                .Timeout(_timeout)
                .FirstAsync()
                .PublishLast();

            using (response.Connect())
            {
                ExecuteCommand(command.Command);

                return await response;
            }
        }

        public abstract Task<IEnumerable<ItemValue>> GetItemValuesAsync(IEnumerable<ItemMetadata> itemNumbers);
        public abstract Task<ItemValue> GetItemValue(ItemMetadata itemNumber);

        protected void PublishStatusMessage(string message)
        {
            var formattedMessage = $"[{CommPort.Name}] {message}";
            StatusObservable.OnNext(formattedMessage);
        }

        protected virtual async Task TryConnectAsync(int retryAttempts = MaxConnectionAttempts,
            TimeSpan? timeout = null)
        {
            if (IsConnected)
                return;

            if (DeviceType == null)
                throw new NullReferenceException(
                    "Device type is not initialized. Call this method with the device type first.");

            if (CancellationTokenSource == null || CancellationTokenSource.IsCancellationRequested)
                CancellationTokenSource = new CancellationTokenSource();

            timeout = timeout.HasValue ? timeout.Value : _timeout;

            Exception exception = null;

            var attempts = 1;
            do
            {
                PublishStatusMessage($"Connecting to {DeviceType.Name}... {attempts} of {MaxConnectionAttempts}");

                if (CancellationToken.IsCancellationRequested)
                    CancellationToken.ThrowIfCancellationRequested();

                if (!CommPort.IsOpen())
                    await CommPort.Open(CancellationToken);

                try
                {
                    await EstablishConnectionAsync(CancellationToken);
                }
                catch (EvcResponseException responseException)
                {
                    PublishStatusMessage(
                        $"Failed connecting to {DeviceType.Name}: Response Message {responseException.Message}.");
                    exception = responseException;
                }
                catch (Exception ex)
                {
                    PublishStatusMessage(
                        $"Failed connecting to {DeviceType.Name}. {Environment.NewLine} Exception: {ex.Message}.");
                    exception = ex;
                }

                if (!IsConnected && attempts <= retryAttempts)
                    await Task.Delay(ConnectionRetryDelayMs, CancellationToken);

                attempts++;
            } while (attempts <= retryAttempts && !IsConnected);

            if (!IsConnected && exception != null)
                throw exception;
            //throw new FailedConnectionException(CommPort, deviceType, retryAttempts, exception);
        }

        #endregion

        #region Private

        private void SetupStreams()
        {
            var incoming = ResponseProcessors.MessageProcessor.ResponseObservable(CommPort.DataReceived)
                .Select(msg => $"[R] - {ControlCharacters.Prettify(msg)}");

            var outgoing = CommPort.DataSent
                .Select(msg => $"[W] - {ControlCharacters.Prettify(msg)}");

            _messagingStream = incoming
                .Merge(outgoing)
                .Publish();

            _messagingConnection = _messagingStream.Connect();

            _statusConnectableObservable = StatusObservable.Publish();
            _statusConnection = _statusConnectableObservable.Connect();

        }

        #endregion
    }
}