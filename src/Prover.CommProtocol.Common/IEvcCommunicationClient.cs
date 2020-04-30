using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Interfaces;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.Common
{
    public interface IEvcCommunicationClient
    {
        IEvcDevice InstrumentType { get; set; }
        bool IsConnected { get; }
        List<ItemMetadata> ItemDetails { get; }
        IObservable<string> Status { get; }

        Task Connect(CancellationToken ct, string accessCode = null, int retryAttempts = 10);
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
    }
}