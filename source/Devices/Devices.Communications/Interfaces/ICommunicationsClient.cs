using Devices.Core.Interfaces;
using Devices.Core.Items;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Communications.Status;
using Prover.Shared.IO;

namespace Devices.Communications.Interfaces
{
    public interface ICommunicationsClient
    {
        bool IsConnected { get; }
        IObservable<StatusMessage> StatusMessageObservable { get; }
        void Cancel();
        Task ConnectAsync(int retryAttempts = 10, TimeSpan? timeout = null);
        Task Disconnect();
        void Dispose();
        Task<IEnumerable<ItemValue>> GetItemsAsync(IEnumerable<ItemMetadata> itemNumbers);
        Task<ItemValue> LiveReadItemValue(ItemMetadata itemNumber);

        Task<bool> SetItemValue(int itemNumber, string value);
        //Task<bool> SetItemValue(ItemMetadata item, string value);
    }

    public interface ICommunicationsClient<out TDevice> : ICommunicationsClient 
        where TDevice : DeviceType
    {
        TDevice DeviceType { get; }
    }

    public static class CommunicationsClientEx
    {
        public static async Task<bool> SetItemValue(this ICommunicationsClient commClient, int itemNumber, long value)
        {
            return await Task.FromResult(true);
        }
    }
}
