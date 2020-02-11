using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;

namespace Devices.Communications.Interfaces
{

    public interface ICommunicationsClient<out TDevice, TInstance>
        where TDevice : DeviceType
        where TInstance : DeviceInstance
    {
        ICommPort CommPort { get; }

        TDevice DeviceType { get; }

        bool IsConnected { get; }

        IObservable<string> StatusMessages { get; }

        void Cancel();

        Task ConnectAsync(int retryAttempts = 10, TimeSpan? timeout = null);

        Task Disconnect();

        void Dispose();

        Task<IEnumerable<ItemValue>> GetItemsAsync(IEnumerable<ItemMetadata> itemNumbers);

        Task<IEnumerable<ItemValue>> GetItemsAsync();

        Task<T> GetItemsAsync<T>() where T : ItemGroup;

        Task<ItemValue> LiveReadItemValue(int itemNumber);
        Task LiveReadItemValue(int itemNumber, IObserver<ItemValue> updates, CancellationToken ct);

        Task<bool> SetItemValue(int itemNumber, decimal value);

        Task<bool> SetItemValue(int itemNumber, long value);

        Task<bool> SetItemValue(int itemNumber, string value);
    }
}

//public interface IDeviceConnection<T>
//  where T : IDeviceType
//{
//    T DeviceType { get; }
//public interface ICommunicationsClient
//{
//    ICommPort CommPort { get; }

//    IDeviceType DeviceType { get; }

//    IDeviceInstance DeviceInstance { get; }

//    bool IsConnected { get; }

//    IObservable<string> StatusMessages { get; }

//    void Cancel();

//    Task ConnectAsync(int retryAttempts = 10, TimeSpan? timeout = null);

//    Task Disconnect();

//    void Dispose();

//    Task<IDeviceInstance> GetDeviceAsync();

//    Task<IEnumerable<ItemValue>> GetItemsAsync(IEnumerable<ItemMetadata> itemNumbers);

//    Task<IEnumerable<ItemValue>> GetItemsAsync();

//    Task<T> GetItemsAsync<T>() where T : IItemsGroup;

//    //Task<ItemValue> LiveReadItemValue(int itemNumber);

//    //Task<bool> SetItemValue(int itemNumber, decimal value);

//    //Task<bool> SetItemValue(int itemNumber, long value);

//    //Task<bool> SetItemValue(int itemNumber, string value);

//    //Task<bool> SetItemValue(string itemCode, long value);
//}