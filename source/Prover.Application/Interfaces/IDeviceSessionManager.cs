using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;

namespace Prover.Application.Interfaces
{
    public interface IActiveDeviceSessionManager : IDeviceSessionManager
    {
        bool Active { get; set; }
    }

    public interface IDeviceSessionManager
    {
        DeviceInstance Device { get; }

        bool SessionInProgress { get; }
        Task Connect();
        Task Disconnect();

        Task<ICollection<ItemValue>> DownloadCorrectionItems();

        Task<ICollection<ItemValue>> DownloadCorrectionItems(ICollection<ItemMetadata> items);

        Task EndSession();
        ICollection<ItemMetadata> GetItemsToDownload();
        Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemsToDownload = null);

        Task<ItemValue> LiveReadItemValue(ItemMetadata item);

        Task<DeviceInstance> StartSession(DeviceType deviceType);

        Task<ItemValue> WriteItemValue(ItemMetadata item, string value);
    }
}