using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using Prover.Core.Shared.Enums;

namespace Prover.CommProtocol.Common.Models.Instrument
{
    public interface IEvcDevice
    {
        int AccessCode { get; set; }
        string Name { get; set; }
        int Id { get; set; }
        string ItemFilePath { get; set; }

        EvcCommunicationClient CommClient { get; }
        
        EvcCorrectorType CorrectorType { get; }
        IEnumerable<ItemMetadata> ItemDefinitions { get; }        

        IPressureItems PressureItems { get; }
        ISiteInformationItems SiteInformationItems { get; }
        ISuperFactorItems SuperFactorItems { get; }
        ITemperatureItems TemperatureItems { get; }
        IVolumeItems VolumeItems { get; }

        Task GetAllItems();

        Task<IPressureItems> GetPressureItems();
        IPressureItems GetPressureItems(Dictionary<string, string> itemData);

        Task<ITemperatureItems> GetTemperatureItems();
        ITemperatureItems GetTemperatureItems(Dictionary<string, string> itemData);

        Task<IVolumeItems> GetVolumeItems();
        IVolumeItems GetVolumeItems(Dictionary<string, string> itemData);
    }
}