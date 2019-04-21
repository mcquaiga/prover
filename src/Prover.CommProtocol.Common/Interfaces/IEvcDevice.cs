using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.Interfaces
{
    public interface IEvcDevice : IEvcDeviceInfo
    {
        IEvcCommunicationClient CommClient { get; }
        bool? CanUseIrDaPort { get; set; }        
        string CommClientType { get; set; }
        int AccessCode { get; set; }

        int? MaxBaudRate { get; set; }

        EvcCommunicationClient CreateCommClient(ICommPort commPort);

        Task GetAllItems();

        Task<IPressureItems> GetPressureItems();

        IPressureItems GetPressureItems(Dictionary<string, string> itemData);

        Task<ITemperatureItems> GetTemperatureItems();

        ITemperatureItems GetTemperatureItems(Dictionary<string, string> itemData);

        Task<IVolumeItems> GetVolumeItems();

        IVolumeItems GetVolumeItems(Dictionary<string, string> itemData);
    }
}
