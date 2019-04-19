using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.Models.Instrument
{
    public interface IEvcDevice
    {
        #region Public Properties

        int AccessCode { get; set; }
        EvcCommunicationClient CommClient { get; }
        EvcCorrectorType CorrectorType { get; }
        int Id { get; set; }
        IEnumerable<ItemMetadata> ItemDefinitions { get; }
        string ItemFilePath { get; }
        string Name { get; set; }
        IPressureItems PressureItems { get; }
        ISiteInformationItems SiteInformationItems { get; }
        ISuperFactorItems SuperFactorItems { get; }
        ITemperatureItems TemperatureItems { get; }
        IVolumeItems VolumeItems { get; }

        #endregion Public Properties

        #region Public Methods

        Task GetAllItems();

        Task<IPressureItems> GetPressureItems();

        IPressureItems GetPressureItems(Dictionary<string, string> itemData);

        Task<ITemperatureItems> GetTemperatureItems();

        ITemperatureItems GetTemperatureItems(Dictionary<string, string> itemData);

        Task<IVolumeItems> GetVolumeItems();

        IVolumeItems GetVolumeItems(Dictionary<string, string> itemData);

        #endregion Public Methods
    }
}