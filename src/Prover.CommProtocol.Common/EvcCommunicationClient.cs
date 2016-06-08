using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Messaging;

namespace Prover.CommProtocol.Common
{
    public abstract class EvcCommunicationClient : IDisposable
    {
        protected const int MaxConnectionAttempts = 10;
        private readonly IDisposable _receivedObservable;
        private readonly IDisposable _sentObservable;
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     A client to communicate with a wide range of EVCs
        /// </summary>
        /// <param name="commPort">Communcations interface to the device</param>
        protected EvcCommunicationClient(CommPort commPort)
        {
            CommPort = commPort;

            _receivedObservable = ResponseProcessors.MessageProcessor.ResponseObservable(CommPort.DataReceivedObservable)
                .Subscribe(msg => { Log.Debug($"[{CommPort.Name}][IN] << {ControlCharacters.Prettify(msg)}"); });

            _sentObservable =
                CommPort.DataSentObservable.Subscribe(
                    msg => { Log.Debug($"[{CommPort.Name}][OUT] >> {ControlCharacters.Prettify(msg)}"); });
        }

        protected CommPort CommPort { get; }

        /// <summary>
        ///     Contains all the item numbers and meta data for a specific instrument type
        /// </summary>
        public virtual IEnumerable<ItemMetadata> ItemDetails { get; } = new List<ItemMetadata>();

        /// <summary>
        ///     Is this client already connected to an instrument
        /// </summary>
        public abstract bool IsConnected { get; protected set; }

        public virtual void Dispose()
        {
            Disconnect().Wait();

            _receivedObservable.Dispose();
            _sentObservable.Dispose();
            CommPort.Dispose();
        }

        private async Task ExecuteCommand(string command)
        {
            await Task.Run(() => CommPort.Send(command));
        }

        protected virtual async Task<T> ExecuteCommand<T>(CommandDefinition<T> command) where T : ResponseMessage
        {
            if (command.ResponseProcessor == null)
            {
                await ExecuteCommand(command.Command);
                return default(T);
            }

            var response = command.ResponseProcessor.ResponseObservable(CommPort.DataReceivedObservable)
                .Timeout(TimeSpan.FromSeconds(3))
                .FirstAsync()
                .PublishLast();

            using (response.Connect())
            {
                CommPort.Send(command.Command);

                var result = await response;
                return result;
            }
        }

        /// <summary>
        ///     Establish a link with an instrument
        ///     Handles retries for failed connections
        /// </summary>
        /// <param name="retryAttempts"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task Connect(int retryAttempts, CancellationTokenSource cancellationToken = null);

        /// <summary>
        ///     Estbalish a link with an instrument
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<bool> Connect(CancellationTokenSource cancellationToken = null);

        /// <summary>
        ///     Disconnect the current link with the EVC, if one exists
        /// </summary>
        public abstract Task Disconnect();

        /// <summary>
        ///     Read item value from instrument
        /// </summary>
        /// <param name="itemNumber">Item number for the value to request</param>
        /// <returns></returns>
        public abstract Task<ItemValue> GetItemValue(int itemNumber);

        /// <summary>
        ///     Read a group of items from instrument
        /// </summary>
        /// <param name="itemNumbers">Item numbers for the values to request</param>
        /// <returns></returns>
        public abstract Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<int> itemNumbers);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        public abstract Task<bool> SetItemValue(int itemNumber, string value);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        public abstract Task<bool> SetItemValue(int itemNumber, decimal value);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        public abstract Task<bool> SetItemValue(int itemNumber, int value);
    }
}