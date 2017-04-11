using Prover.Shared.Enums;

namespace Prover.Domain.Instrument.Items
{
    public interface IVolumeItems : IItemsGroup
    {
        DriveTypeDescripter DriveType { get; }
        double DriveRate { get; }
        string DriveRateDescription { get; }

        double CorrectedMultiplier { get; }
        double CorrectedReading { get; }
        string CorrectedUnits { get; }
        
        double Energy { get; }
        double EnergyGasValue { get; }
        string EnergyUnits { get; }

        double MeterDisplacement { get; }
        string MeterModel { get; }
        int MeterModelId { get; }

        double UncorrectedMultiplier { get; }
        double UncorrectedReading { get; }
        string UncorrectedUnits { get; }
    }
}