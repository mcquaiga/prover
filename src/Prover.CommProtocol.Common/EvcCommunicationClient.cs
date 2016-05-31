using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Messaging;

namespace Prover.CommProtocol.Common
{
    public abstract class EvcCommunicationClient
    {
        protected const int MaxConnectionAttempts = 10;
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     A client to communicate with a wide range of EVCs
        /// </summary>
        /// <param name="commPort">Communcations interface to the device</param>
        protected EvcCommunicationClient(CommPort commPort)
        {
            CommPort = commPort;
        }

        protected virtual async Task ExecuteCommand(string cmd)
        {
            await CommPort.SendAsync(cmd);
        }

        protected virtual async Task<T> ExecuteCommand<T>(string cmd, ResponseProcessor<T> processor)
        { 
            if (processor == null) throw new ArgumentNullException(nameof(processor));

            var response = processor.ResponseObservable(CommPort.ReceivedSubject)
                            .Timeout(TimeSpan.FromSeconds(2))
                            .FirstAsync()
                            .PublishLast();
            response.Connect();

            await CommPort.SendAsync(cmd);

            var result = await response;
            Log.Debug($"Response -> {result.ToString()}");
            return result;
        }

        protected CommPort CommPort { get; }

        /// <summary>
        /// Contains all the item numbers and meta data for a specific instrument type
        /// </summary>
        public virtual IEnumerable<ItemMetadata> ItemDetails { get; } = new List<ItemMetadata>();

        /// <summary>
        ///     Is there already a connection open
        /// </summary>
        public abstract bool IsConnected { get; protected set; }

        /// <summary>
        ///     Establish a connection through the CommPort with an EVC
        /// </summary>
        public abstract Task Connect(int retryAttempts = MaxConnectionAttempts, CancellationTokenSource cancellationToken = null);

        /// <summary>
        ///     Disconnect the current link with the EVC, if one exists
        /// </summary>
        public abstract Task Disconnect();

        /// <summary>
        ///     Read item value from instrument
        /// </summary>
        /// <param name="itemNumber">Item number for the value to request</param>
        /// <returns></returns>
        public abstract Task<object> GetItemValue(int itemNumber);

        /// <summary>
        ///     Read a group of items from instrument
        /// </summary>
        /// <param name="itemNumbers">Item numbers for the values to request</param>
        /// <returns></returns>
        public abstract Task<object> GetItemValue(IEnumerable<int> itemNumbers);

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