using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        private const int ConnectionRetryDelayMs = 3000;
        private const int MaxConnectionAttempts = 10;
        private IDisposable _receivedObservable;
        private IDisposable _sentObservable;
        private readonly Subject<string> _statusSubject = new Subject<string>();

        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static EvcCommunicationClient Create(InstrumentType instrumentType, ICommPort commPort)
        {
            return instrumentType.ClientFactory.Invoke(commPort, null);
        }

        /// <summary>
        ///     A client to communicate with a wide range of EVCs
        /// </summary>
        /// <param name="commPort">Communcations interface to the device</param>
        /// <param name="instrumentType">Instrument type of device</param>
        /// <param name="statusSubject">Subject for listening to status updates</param>
        protected EvcCommunicationClient(ICommPort commPort, InstrumentType instrumentType, ISubject<string> statusSubject = null)
        {
            CommPort = commPort;
            InstrumentType = instrumentType;

            if (statusSubject != null)
                Status?.Subscribe(statusSubject);

            Status?.Subscribe(s => Log.Debug(s));

            _receivedObservable = ResponseProcessors.MessageProcessor.ResponseObservable(CommPort.DataReceivedObservable)
                .Subscribe(msg => { Log.Debug($"[{CommPort.Name}] [R] {ControlCharacters.Prettify(msg)}"); });

            _sentObservable = CommPort.DataSentObservable
                .Subscribe(msg => { Log.Debug($"[{CommPort.Name}] [S] {ControlCharacters.Prettify(msg)}"); });
        }

        protected ICommPort CommPort { get; }

        public InstrumentType InstrumentType { get; set; }

        public IObservable<string> Status => _statusSubject.AsObservable();

        /// <summary>
        ///     Contains all the item numbers and meta data for a specific instrument type
        /// </summary>
        public virtual List<ItemMetadata> ItemDetails { get; protected set; } = new List<ItemMetadata>();

        /// <summary>
        ///     Is this client already connected to an instrument
        /// </summary>
        public abstract bool IsConnected { get; protected set; }        

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
                .Timeout(TimeSpan.FromSeconds(5))
                .FirstAsync()
                .PublishLast();

            using (response.Connect())
            {
                await CommPort.Send(command.Command);

                var result = await response;

                return result;
            }
        }

        /// <summary>
        ///     Establish a link with an instrument
        ///     Handles retries for failed connections
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <param name="accessCode"></param>
        /// <param name="retryAttempts"></param>
        /// <returns></returns>
        public async Task Connect(CancellationToken ct, string accessCode = null, int retryAttempts = MaxConnectionAttempts)
        {
            if (IsConnected)
                return;

            var connectionAttempts = 1;

            ct.ThrowIfCancellationRequested();

            var result = Task.Run(async () =>
            {
                while (!IsConnected)
                {
                    _statusSubject.OnNext($"Connecting to {InstrumentType.Name} on {CommPort.Name}... {connectionAttempts} of {MaxConnectionAttempts}");

                    if (ct.IsCancellationRequested)
                        ct.ThrowIfCancellationRequested();
                    
                    if (!CommPort.IsOpen())
                        await CommPort.Open(ct);

                    try
                    {
                        await ConnectToInstrument(ct, accessCode);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Log.Warn(ex);
                    }

                    if (!IsConnected)
                    {
                        if (connectionAttempts < retryAttempts)
                        {
                            Log.Warn($"[{CommPort.Name}] Failed connecting to {InstrumentType.Name}.");
                            Thread.Sleep(ConnectionRetryDelayMs);
                            connectionAttempts++;
                        }
                        else
                            throw new Exception($"{CommPort.Name} Could not connect to {InstrumentType.Name} after {retryAttempts} retries.");
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
                    Log.Warn(e.Message + " " + v.Message);
            }
        }

        /// <summary>
        ///     Establish a link with an instrument
        ///     Handles retries for failed connections
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="accessCode"></param>
        /// <returns></returns>
        protected abstract Task ConnectToInstrument(CancellationToken ct, string accessCode = null);

        /// <summary>
        ///     Disconnect the current link with the EVC, if one exists
        /// </summary>
        public abstract Task Disconnect();

        /// <summary>
        ///     Read item value from instrument
        /// </summary>
        /// <param name="itemNumber">Item number for the value to request</param>
        /// <returns></returns>
        public abstract Task<ItemValue> GetItemValue(ItemMetadata itemNumber);

        ///// <summary>
        /////     Read a group of items from instrument
        ///// </summary>
        ///// <param name="itemNumbers">Item numbers for the values to request</param>
        ///// <returns></returns>
        //public abstract Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<int> itemNumbers);

        /// <summary>
        ///     Read a group of items from instrument
        /// </summary>
        /// <param name="itemNumbers">Item numbers for the values to request</param>
        /// <returns></returns>
        public abstract Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers);

        /// <summary>
        ///     Read all items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ItemValue>> GetAllItems()
        {
            _statusSubject.OnNext("Downloading items...");
            return await GetItemValues(InstrumentType.ItemsMetadata);
        }

        /// <summary>
        ///     Read volume items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ItemValue>> GetVolumeItems()
        {
            return await GetItemValues(InstrumentType.ItemsMetadata.VolumeItems());
        }

        /// <summary>
        ///     Read frequency test items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public abstract Task<IFrequencyTestItems> GetFrequencyItems();       

         /// <summary>
        ///     Read temperature test items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ItemValue>> GetPulseOutputItems()
        {
            return await GetItemValues(ItemDetails.PulseOutputItems());
        }

        /// <summary>
        ///     Read pressure test items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ItemValue>> GetPressureTestItems()
        {
            return await GetItemValues(InstrumentType.ItemsMetadata.PressureItems());
        }

        /// <summary>
        ///     Read temperature test items defined in items xml definitions
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ItemValue>> GetTemperatureTestItems()
        {
            return await GetItemValues(InstrumentType.ItemsMetadata.TemperatureItems());
        }

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
        public abstract Task<bool> SetItemValue(int itemNumber, long value);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract Task<bool> SetItemValue(string itemCode, long value);

        /// <summary>
        ///     Live read item values
        ///     Gas Temp / Gas Pressure
        /// </summary>
        /// <param name="itemNumber">Item number to live read</param>
        /// <returns></returns>
        public abstract Task<ItemValue> LiveReadItemValue(int itemNumber);

        public virtual void Dispose()
        {
            Disconnect().ContinueWith(task =>
            {
                _receivedObservable?.Dispose();
                _sentObservable?.Dispose();
                _statusSubject?.Dispose();
                CommPort?.Dispose();
            });
        }
    }
}