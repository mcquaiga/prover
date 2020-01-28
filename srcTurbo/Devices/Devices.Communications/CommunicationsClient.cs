using System;
using System.Collections.Generic;
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
    public abstract class CommunicationsClient : ICommunicationsClient, IDisposable
    {
        private const int ConnectionRetryDelayMs = 3000;
        private const int MaxConnectionAttempts = 10;

        /// <summary>
        ///     A client to communicate with a wide range of EVCs
        /// </summary>
        /// <param name="commPort">Communcations interface to the device</param>
        /// <param name="deviceType"></param>
        /// <param name="EvcDeviceType">Instrument type of device</param>
        /// <param name="statusSubject">Subject for listening to status updates</param>
        protected CommunicationsClient(ICommPort commPort, IDeviceType deviceType)
        {
            CommPort = commPort;
            DeviceType = deviceType;
            SetupStreams();
        }

        private readonly IDisposable _receivedObservable;
        private readonly IDisposable _sentObservable;
        
        protected readonly ISubject<string> StatusStream = new Subject<string>();
        private TimeSpan _timeout = TimeSpan.FromSeconds(5);
        protected virtual CancellationToken CancellationToken => CancellationTokenSource.Token;

        protected virtual CancellationTokenSource CancellationTokenSource { get; private set; } =
            new CancellationTokenSource();

        public ICommPort CommPort { get; }

        public virtual IDeviceInstance DeviceInstance { get; protected set; }

        public IDeviceType DeviceType { get; }

        /// <summary>
        ///     Is this client already connected to an instrument
        /// </summary>
        public abstract bool IsConnected { get; protected set; }

        public IObservable<string> Status { get; private set; }

        public Task Cancel()
        {
            return Task.Run(CancellationTokenSource.Cancel);
        }

        public Task ConnectAsync(int retryAttempts = MaxConnectionAttempts, TimeSpan? timeout = null)
        {
            return TryConnectAsync(DeviceType, retryAttempts, timeout);
        }

        /// <summary>
        ///     Disconnect the current link with the EVC, if one exists
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

        public Task<IEnumerable<ItemValue>> GetItemsAsync(IEnumerable<ItemMetadata> itemNumbers)
        {
            StatusStream.OnNext("Downloading items...");
            return GetItemValuesAsync(itemNumbers);
        }

        public Task<IEnumerable<ItemValue>> GetItemsAsync()
        {
            StatusStream.OnNext("Downloading items...");
            return GetItemValuesAsync(DeviceType.Items);
        }

        public async Task<T> GetItemsAsync<T>()
            where T : IItemsGroup
        {
            var items = DeviceType.GetItemMetadataByGroup<T>();
            var values = await GetItemValuesAsync(items);
            return DeviceInstance.GetItemsByGroup<T>(values);
        }

        public Task<IDeviceInstance> GetDeviceAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IDeviceInstance> GetDeviceAsync(IDeviceType deviceType)
        {
            var values = await GetItemsAsync();
            DeviceInstance = deviceType.CreateDeviceInstance(values);
            return DeviceInstance;
        }

        protected virtual async Task TryConnectAsync(IDeviceType deviceType, int retryAttempts = MaxConnectionAttempts,
            TimeSpan? timeout = null)
        {
            if (IsConnected)
                return;

            if (deviceType == null)
                throw new NullReferenceException(
                    "Device type is not initialized. Call this method with the device type first.");

            if (CancellationTokenSource == null || CancellationTokenSource.IsCancellationRequested)
                CancellationTokenSource = new CancellationTokenSource();

            _timeout = timeout.HasValue ? timeout.Value : _timeout;

            Exception exception = null;

            var attempts = 1;
            do
            {
                StatusStream.OnNext(
                    $"Connecting to {deviceType.Name} on {CommPort.Name}... {attempts} of {MaxConnectionAttempts}");

                if (CancellationToken.IsCancellationRequested)
                    CancellationToken.ThrowIfCancellationRequested();

                if (!CommPort.IsOpen())
                    await CommPort.Open(CancellationToken);

                try
                {
                    await EstablishConnectionAsync(deviceType, CancellationToken);
                }
                catch (EvcResponseException responseException)
                {
                    StatusStream.OnNext($"[{CommPort.Name}] Failed connecting to {deviceType.Name}: Response Message {responseException.Message}.");
                    exception = responseException;
                }
                catch (Exception ex)
                {
                    StatusStream.OnNext($"[{CommPort.Name}] Failed connecting to {deviceType.Name}.");
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

        protected abstract Task EstablishConnectionAsync<T>(T deviceType, CancellationToken ct) where T : IDeviceType;

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

        protected abstract Task<IEnumerable<ItemValue>> GetItemValuesAsync(IEnumerable<ItemMetadata> itemNumbers);

        private void SetupStreams()
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
                    StatusStream.OnNext(msg);
                    Console.WriteLine(msg);
                });

            Status = StatusStream.Publish();
            var statusDisposable = (Status as IConnectableObservable<string>).Connect();

            incoming.Connect();
            outgoing.Connect();
            chatStream.Connect();

            //Status.Subscribe(
                //s => ,
                //() => statusDisposable.Dispose());
        }
    }
}