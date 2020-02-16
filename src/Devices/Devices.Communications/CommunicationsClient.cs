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
using Devices.Communications.Status;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;

namespace Devices.Communications
{
    public abstract class CommunicationsClient : ICommunicationsClient, IDisposable
    {
        private const int ConnectionRetryDelayMs = 3000;
        private const int MaxConnectionAttempts = 10;

        private readonly ISubject<StatusMessage> _statusSubject = new Subject<StatusMessage>();

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(3);

        private CompositeDisposable _cleanup;

        /// <summary>
        ///     A client to communicate with a wide range of EVCs
        /// </summary>
        /// <param name="commPort">Communcations interface to the device</param>
        /// <param name="deviceType"></param>
        /// <param name="EvcDeviceType">Instrument type of device</param>
        /// <param name="statusSubject">Subject for listening to status updates</param>
        protected CommunicationsClient(ICommPort commPort, DeviceType deviceType)
        {
            CommPort = commPort;

            DeviceType = deviceType;

            var disposable = ResponseProcessors.MessageProcessor.ResponseObservable(CommPort.DataReceived).Select(Messages.DataRecieved)
                .Merge(CommPort.DataSent.Select(Messages.DataSent))
                .Subscribe(_statusSubject);

            var status = _statusSubject.Publish();
            StatusMessageObservable = status.AsObservable();

            _cleanup = new CompositeDisposable(disposable, status.Connect());
        }


        public IObservable<StatusMessage> StatusMessageObservable { get; }

        #region Private

        #endregion

        #region Public Properties

        /// <summary>
        ///     Is this client already connected to an instrument
        /// </summary>
        public abstract bool IsConnected { get; protected set; }

        public IObservable<string> StatusMessages { get; }

        public ICommPort CommPort { get; }

        public DeviceType DeviceType { get; }
        #endregion

        #region Public Methods

        public void Cancel()
        {
            try
            {
                CancellationTokenSource.Cancel(false);
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions) Console.WriteLine($"Exception {ex.Message}");
            }
        }

        public Task ConnectAsync(int retryAttempts = MaxConnectionAttempts, TimeSpan? timeout = null)
        {
            return TryConnectAsync(retryAttempts, timeout);
        }

        /// <summary>
        ///     Disconnect the current link with the EVC, if one exists
        /// </summary>
        public async Task Disconnect()
        {
            await TryDisconnect();
            //_messagingConnection.Dispose();
        }

        public async Task<IEnumerable<ItemValue>> GetItemsAsync(IEnumerable<ItemMetadata> itemNumbers)
        {
            return await GetItemValuesAsync(itemNumbers);
        }

        public async Task<IEnumerable<ItemValue>> GetItemsAsync()
        {
            return await GetItemValuesAsync(DeviceType.Items);
        }

        public async Task<T> GetItemsAsync<T>()
            where T : ItemGroup
        {
            var items = DeviceType.GetItemMetadata<T>();
            var values = await GetItemValuesAsync(items);
            return DeviceType.GetGroupValues<T>(values);
        }

        public virtual void Dispose()
        {
            Disconnect().Wait(TimeSpan.FromMilliseconds(500));
            _statusSubject.OnCompleted();
            _cleanup.Dispose();
            CommPort?.Dispose();
        }

        public abstract Task<ItemValue> GetItemValue(ItemMetadata itemNumber);

        public abstract Task<IEnumerable<ItemValue>> GetItemValuesAsync(IEnumerable<ItemMetadata> itemNumbers);

        public abstract Task<ItemValue> LiveReadItemValue(int itemNumber);

        public abstract Task LiveReadItemValue(int itemNumber, IObserver<ItemValue> updates,
            CancellationToken ct);

        public abstract Task<bool> SetItemValue(int itemNumber, decimal value);
        public abstract Task<bool> SetItemValue(int itemNumber, long value);

        public abstract Task<bool> SetItemValue(int itemNumber, string value);

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

        protected void PublishStatusMessage(string message)
        {
            //var formattedMessage = $"[{CommPort.Name}] {message}";
            //_statusSubject.OnNext(formattedMessage);
        }

        protected void PublishMessage(StatusMessage message)
        {
            _statusSubject.OnNext(message);
        }

        protected virtual async Task TryConnectAsync(int retryAttempts = MaxConnectionAttempts,
            TimeSpan? timeout = null)
        {
            if (IsConnected)
                return;

            //SetupStreams();

            if (CancellationTokenSource == null || CancellationTokenSource.IsCancellationRequested)
                CancellationTokenSource = new CancellationTokenSource();

            CancellationToken.ThrowIfCancellationRequested();

            timeout = timeout ?? _timeout;

            Exception exception = null;

            var attempts = 1;
            do
            {
                PublishMessage(Messages.Info($"Connecting to {DeviceType.Name}... {attempts} of {MaxConnectionAttempts}"));

                if (!CommPort.IsOpen())
                    await CommPort.Open(CancellationToken);

                try
                {
                    await EstablishConnectionAsync(CancellationToken);
                }
                catch (EvcResponseException responseException)
                {
                    PublishMessage(Messages.Error($"Failed connecting to {DeviceType.Name}: Response Message {responseException.Message}."));
                    exception = responseException;
                }
                catch (OperationCanceledException)
                {
                    PublishMessage(Messages.Error($"Failed connecting to {DeviceType.Name}: Operation canceled."));
                }
                catch (Exception ex)
                {
                    PublishMessage(Messages.Error($"Failed connecting to {DeviceType.Name}. {Environment.NewLine} Exception: {ex.Message}."));
                    exception = ex;
                }

                if (!IsConnected && attempts <= retryAttempts)
                    await Task.Delay(ConnectionRetryDelayMs, CancellationToken);

                attempts++;
            } while (attempts <= retryAttempts && !IsConnected && !CancellationToken.IsCancellationRequested);

            if (!IsConnected && exception != null)
                throw exception;
            //throw new FailedConnectionException(CommPort, deviceType, retryAttempts, exception);
        }

        protected abstract Task TryDisconnect();

        #endregion
    }
}