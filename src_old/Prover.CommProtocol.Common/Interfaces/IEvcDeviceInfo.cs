using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using System.Collections.Generic;

namespace Prover.CommProtocol.Common.Interfaces
{
    public interface IEvcDeviceInfo
    {
        #region Public Properties

        EvcCorrectorType CorrectorType { get; }
        int Id { get; set; }
        bool IsHidden { get; set; }
        IEnumerable<ItemMetadata> ItemDefinitions { get; }
        IEnumerable<ItemValue> ItemValues { get; }
        string Name { get; set; }
        IPressureItems PressureItems { get; }
        ISiteInformationItems SiteInformationItems { get; }
        ISuperFactorItems SuperFactorItems { get; }
        ITemperatureItems TemperatureItems { get; }
        IVolumeItems VolumeItems { get; }

        #endregion Public Properties

        #region Public Methods



        TValue GetValue<TValue>(string code) where TValue : struct;



        #endregion Public Methods
    }
}