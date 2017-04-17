using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Prover.InstrumentProtocol.Core.Items;

namespace Prover.InstrumentProtocol.Core
{
    public interface IEvcCommunicationClient
    {
        /// <summary>
        ///     Is this client already connected to an instrument
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        ///     Establish a link with an instrument
        ///     Handles retries for failed connections
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <param name="retryAttempts"></param>
        /// <returns></returns>
        Task Connect(CancellationToken ct, int retryAttempts = 0);

        /// <summary>
        ///     Disconnect the current link with the EVC, if one exists
        /// </summary>
        Task Disconnect();

        void Dispose();

        /// <summary>
        ///     Read item value from instrument
        /// </summary>
        /// <param name="itemNumber">Item number for the value to request</param>
        /// <returns></returns>
        Task<ItemValue> GetItemValue(int itemNumber);

        /// <summary>
        ///     Read a group of items from instrument
        /// </summary>
        /// <param name="itemNumbers">Item numbers for the values to request</param>
        /// <returns></returns>
        Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<int> itemNumbers);

        /// <summary>
        ///     Read a group of items from instrument
        /// </summary>
        /// <param name="itemNumbers">Item numbers for the values to request</param>
        /// <returns></returns>
        Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers);

        /// <summary>
        ///     Live read item values
        ///     Gas Temp / Gas Pressure
        /// </summary>
        /// <param name="item">Item number to live read</param>
        /// <returns></returns>
        Task<ItemValue> LiveReadItemValue(ItemMetadata item);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        Task<bool> SetItemValue(int itemNumber, string value);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        Task<bool> SetItemValue(int itemNumber, decimal value);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemNumber">Item number to write value</param>
        /// <param name="value">Value to write</param>
        /// <returns>True - Successful, False - Failed to write</returns>
        Task<bool> SetItemValue(int itemNumber, long value);

        /// <summary>
        ///     Write a value to an item
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> SetItemValue(string itemCode, long value);
    }
}