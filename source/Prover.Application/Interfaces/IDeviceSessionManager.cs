using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
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

        Task<IDeviceSessionManager> StartSession(DeviceType deviceType, string commPortName, int baudRate,
            ReactiveObject owner);
    }
}