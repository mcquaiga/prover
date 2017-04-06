using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Domain.Models.Instruments.Items;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.Instruments
{
    public interface IInstrument
    {
        int Id { get; }
        string Name { get; }

        Dictionary<string, string> ItemData { get; }
        EvcCorrectorType CorrectorType { get; }

        ISiteInformationItems SiteInformationItems { get; }
        ITemperatureItems TemperatureItems { get; }
        IPressureItems PressureItems { get; }
        ISuperFactorItems SuperFactorItems { get; }
        IVolumeItems VolumeItems { get; }

        Task<T> GetItemsByGroup<T>(T itemGroup);

        Task<ITemperatureItems> GetTemperatureItems();
        Task<IPressureItems> GetPressureItems();
        Task<IVolumeItems> GetVolumeItems();
    }
}