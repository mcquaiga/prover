using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Domain.Instrument.Items;
using Prover.Shared.Enums;

namespace Prover.Domain.Instrument
{
    public interface IInstrument
    {
        EvcCorrectorType CorrectorType { get; }
        int Id { get; }

        Dictionary<string, string> ItemData { get; }
        string Name { get; }
        IPressureItems PressureItems { get; }

        ISiteInformationItems SiteInformationItems { get; }
        ISuperFactorItems SuperFactorItems { get; }
        ITemperatureItems TemperatureItems { get; }
        IVolumeItems VolumeItems { get; }
        Task<IPressureItems> GetPressureItems();

        Task<ITemperatureItems> GetTemperatureItems();
        Task<IVolumeItems> GetVolumeItems();
    }
}