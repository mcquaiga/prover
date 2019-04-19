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
	bool? CanUseIrDaPort { get; set; }
        Func<ICommPort, ISubject<string>, EvcCommunicationClient> ClientFactory { get; set; }
        string CommClientType { get; set; }
        EvcCorrectorType CorrectorType { get; }
        int Id { get; set; }
        HashSet<ItemMetadata> Items { get; }
        IEnumerable<ItemMetadata> ItemDefinitions { get; }
        string ItemFilePath { get; }
        string Name { get; set; }
        int? MaxBaudRate { get; set; }
        string Name { get; set; }
        bool IsHidden { get; set; }
        void LoadItemsInformation();
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