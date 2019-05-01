using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Communications
{
    public interface IEvcCommunicationClient<TEvcType> where TEvcType : IDeviceType
    {
        #region Public Properties

        TEvcType EvcDeviceType { get; set; }

        bool IsConnected { get; }

        List<ItemMetadata> ItemDetails { get; }

        IObservable<string> Status { get; }

        #endregion Public Properties

        #region Public Methods

        Task Connect(CancellationToken ct, int retryAttempts = 10, TimeSpan? timeout = null);

        Task Disconnect();

        void Dispose();

        Task<IEnumerable<ItemValue>> GetAllItems();

        Task<IFrequencyTestItems> GetFrequencyItems();

        Task<ItemValue> GetItemValue(ItemMetadata itemNumber);

        Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers);

        Task<IEnumerable<ItemValue>> GetPressureTestItems();

        Task<IEnumerable<ItemValue>> GetPulseOutputItems();

        Task<IEnumerable<ItemValue>> GetTemperatureTestItems();

        Task<IEnumerable<ItemValue>> GetVolumeItems();

        Task<ItemValue> LiveReadItemValue(int itemNumber);

        Task<bool> SetItemValue(int itemNumber, decimal value);

        Task<bool> SetItemValue(int itemNumber, long value);

        Task<bool> SetItemValue(int itemNumber, string value);

        Task<bool> SetItemValue(string itemCode, long value);

        #endregion Public Methods
    }
}