using Prover.Shared.Enums;

namespace Prover.Domain.Models.Instruments.Items
{
    public interface IVolumeItems
    {
        decimal UncorrectedReading { get; }
        decimal UncorrectedMultiplier { get; }
        string UncorrectedUnits { get; }
        decimal CorrectedReading { get; }
        decimal CorrectedMultiplier { get; }
        string CorrectedUnits { get; }

        DriveTypeDescripter DriveType { get; }
        decimal DriveRate { get; }
        string DriveRateDescription { get; }

        int MeterModelId { get; }
        string MeterModel { get; }
        decimal MeterDisplacement { get; }

        string EnergyUnits { get; }
        decimal Energy { get; }
        decimal EnergyGasValue { get; }
    }
}