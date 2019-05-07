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
        IDeviceWithValues Device { get; }
        IDevice DeviceType { get; }
        bool IsConnected { get; }

        IObservable<string> Status { get; }

        Task Cancel();

        Task ConnectAsync(int retryAttempts = 10, TimeSpan? timeout = null);

        Task Disconnect();

        void Dispose();

        Task<IDeviceWithValues> GetDeviceAsync();

        Task<IEnumerable<ItemValue>> GetItemsAsync(IEnumerable<ItemMetadata> itemNumbers);

        Task<IEnumerable<ItemValue>> GetItemsAsync();

        Task<T> GetItemsAsync<T>() where T : IItemsGroup;

        //Task<IFrequencyTestItems> GetFrequencyItems();

        //Task<ItemValue> GetItemValue(ItemMetadata itemNumber);

        //Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers);

        //Task<IEnumerable<ItemValue>> GetPressureTestItems();

        //Task<IEnumerable<ItemValue>> GetPulseOutputItems();

        //Task<IEnumerable<ItemValue>> GetTemperatureTestItems();

        //Task<IEnumerable<ItemValue>> GetVolumeItems();

        //Task<ItemValue> LiveReadItemValue(int itemNumber);

        //Task<bool> SetItemValue(int itemNumber, decimal value);

        //Task<bool> SetItemValue(int itemNumber, long value);

        //Task<bool> SetItemValue(int itemNumber, string value);

        //Task<bool> SetItemValue(string itemCode, long value);
    }
}