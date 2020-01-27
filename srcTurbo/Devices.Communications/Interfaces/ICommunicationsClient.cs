using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Communications.Interfaces
{
    //public interface IDeviceConnection<T>
    //  where T : IDeviceType
    //{
    //    T DeviceType { get; }

    public interface ICommunicationsClient

    {
        ICommPort CommPort { get; }

        IDeviceInstance Device { get; }
        IDeviceType DeviceType { get; }
        bool IsConnected { get; }

        IObservable<string> Status { get; }

        Task Cancel();

        Task ConnectAsync(int retryAttempts = 10, TimeSpan? timeout = null);

        Task Disconnect();

        void Dispose();

        Task<IDeviceInstance> GetDeviceAsync();

        Task<IEnumerable<ItemValue>> GetItemsAsync(IEnumerable<ItemMetadata> itemNumbers);

        Task<IEnumerable<ItemValue>> GetItemsAsync();

        Task<T> GetItemsAsync<T>() where T : IItemsGroup;

        //Task<ItemValue> LiveReadItemValue(int itemNumber);

        //Task<bool> SetItemValue(int itemNumber, decimal value);

        //Task<bool> SetItemValue(int itemNumber, long value);

        //Task<bool> SetItemValue(int itemNumber, string value);

        //Task<bool> SetItemValue(string itemCode, long value);
    }
}